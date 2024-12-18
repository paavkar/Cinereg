﻿using Cinereg.Data;
using Cinereg.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Cinereg.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SeriesService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<bool> Add(Series series)
        {
            List<string> genreIds = await AddGenres(series);

            series.Seasons.OrderBy(se => se.ReleaseYear).ToList();

            series.Id = Guid.CreateVersion7().ToString();
            series.StartYear = series.Seasons.First().ReleaseYear;
            series.EndYear = series.Seasons.Last().ReleaseYear;

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertSeriesCommand = @"INSERT INTO Series (Id, Name, StartYear, EndYear, ViewingForm, Review, UserId)
                                        VALUES (@Id, @Name, @StartYear, @EndYear, @ViewingForm, @Review, @UserId)";

                        await connection.ExecuteAsync(insertSeriesCommand, series, transaction);


                        foreach (string genreId in genreIds)
                        {
                            string insertGenreCommand = @"INSERT INTO SeriesGenres (GenreId, SeriesId) VALUES (@GenreId, @SeriesId)";
                            await connection.ExecuteAsync(insertGenreCommand, new { GenreId = genreId, SeriesId = series.Id }, transaction);
                        }

                        foreach (Season season in series.Seasons)
                        {
                            season.ShowId = series.Id;

                            string insertSeasonCommand = @"INSERT INTO Seasons (Id, ShowId, SeasonNumber, NumberOfEpisodes, Rating, Review, ReleaseYear, WatchedYear, ViewingForm)
                                        VALUES (@Id, @ShowId, @SeasonNumber, @NumberOfEpisodes, @Rating, @Review, @ReleaseYear, @WatchedYear, @ViewingForm)";

                            await connection.ExecuteAsync(insertSeasonCommand, season, transaction);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            };
        }

        public async Task<List<string>> AddGenres(Series series)
        {
            using var connection = GetConnection();
            string sql = @"SELECT * FROM Genres WHERE Name = @GenreName";

            List<string> genreIds = new();

            foreach (Genre genre in series.SeriesGenres)
            {
                genre.Name = genre.Name.Trim();
                genre.Name = genre.Name.Substring(0, 1).ToUpper() + genre.Name.Substring(1).ToLower();
                var existingGenre = await connection.QueryFirstOrDefaultAsync<Genre>(sql, new { GenreName = genre.Name });

                if (existingGenre is null)
                {
                    string genreId = genre.Id;
                    if (String.IsNullOrEmpty(genre.Id)) genreId = Guid.CreateVersion7().ToString();
                    genreIds.Add(genreId);
                    string insertGenreCommand = @"INSERT INTO Genres (Id, Name) VALUES (@Id, @GenreName)";
                    await connection.ExecuteAsync(insertGenreCommand, new { Id = genreId, GenreName = genre.Name });
                }
                else genreIds.Add(existingGenre.Id);
            }

            return genreIds;
        }

        public async Task<bool> Delete(string id)
        {
            var sql = @"
                    DELETE FROM Series
                    WHERE Id = @Id";

            using var connection = GetConnection();
            int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

            return rowsAffected > 0;
        }

        public async Task<List<Series>> GetAll(string userId)
        {
            var sql = GetSeriesSql();

            var seriesDapper = await QuerySeriesAsync(sql, new { UserId = userId });
            return seriesDapper.Distinct().ToList();
        }

        public async Task<Series> GetById(string id)
        {
            var sql = GetSeriesSql(true);

            var seriesDapper = await QuerySeriesAsync(sql, new { Id = id });
            return seriesDapper.Distinct().ToList().First();
        }

        public async Task<bool> Update(string id, Series series)
        {
            var dbSeries = await GetById(id);
            if (dbSeries == null) return false;

            List<Season> newSeasons = new();
            List<Season> oldSeasons = new();

            foreach (Season season in series.Seasons)
            {
                if (dbSeries.Seasons.Find(s => s.Id == season.Id) is null)
                {
                    newSeasons.Add(season);
                }
                else oldSeasons.Add(season);
            }

            series.EndYear = series.Seasons.Last().ReleaseYear;

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = @"
                                     UPDATE Series
                                     SET Name = @Name,
                                         StartYear = @StartYear,
                                         EndYear = @EndYear,
                                         ViewingForm = @ViewingForm,
                                         Review = @Review
                                     WHERE Id = @Id";
                        await connection.ExecuteAsync(sql, series, transaction);

                        List<string> genreIds = await AddGenres(series);

                        string deleteSql = @"
                                           DELETE FROM SeriesGenres
                                           WHERE GenreId NOT IN @GenreIds
                                           AND SeriesId = @SeriesId";
                        await connection.ExecuteAsync(deleteSql, new { GenreIds = genreIds, SeriesId = id }, transaction);

                        string addSeriesGenresSql = @"
                                                    INSERT INTO SeriesGenres (GenreId, SeriesId) 
                                                    SELECT @GenreId, @SeriesId
                                                    WHERE NOT EXISTS (SELECT 1
                                                                      FROM SeriesGenres
                                                                      WHERE SeriesId = @SeriesId
                                                                      AND GenreId = @GenreId)";

                        foreach (var genreId in genreIds)
                        {
                            await connection.ExecuteAsync(addSeriesGenresSql, new { GenreId = genreId, SeriesId = id }, transaction);
                        }

                        foreach (Season season in oldSeasons)
                        {
                            string updateSeasonCommand = @"
                                                         UPDATE Seasons
                                                         SET SeasonNumber = @SeasonNumber,
                                                             NumberOfEpisodes = @NumberOfEpisodes,
                                                             Rating = @Rating,
                                                             Review = @Review,
                                                             ReleaseYear = @ReleaseYear,
                                                             WatchedYear = @WatchedYear,
                                                             ViewingForm = @ViewingForm
                                                         WHERE Id = @Id";

                            await connection.ExecuteAsync(updateSeasonCommand, season, transaction);
                        }

                        foreach (Season season in newSeasons)
                        {
                            season.ShowId = id;

                            string insertSeasonCommand = @"INSERT INTO Seasons (Id, ShowId, SeasonNumber, NumberOfEpisodes, Rating, Review, ReleaseYear, WatchedYear, ViewingForm)
                                        VALUES (@Id, @ShowId, @SeasonNumber, @NumberOfEpisodes, @Rating, @Review, @ReleaseYear, @WatchedYear, @ViewingForm)";

                            await connection.ExecuteAsync(insertSeasonCommand, season, transaction);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        private string GetSeriesSql(bool singleSeries = false)
        {
            var sql = @"
                    SELECT s.*, g.Id AS GenreId, g.Name, se.Id AS SeasonId, se.*
                    FROM Series s
                    LEFT JOIN SeriesGenres sg ON s.Id = sg.SeriesId
                    LEFT JOIN Genres g ON sg.GenreId = g.Id
                    LEFT JOIN Seasons se ON s.Id = se.ShowId";

            if (singleSeries) sql += " WHERE s.Id = @Id";
            else sql += " WHERE s.UserId = @UserId ORDER BY s.Name";

            return sql;
        }

        private async Task<IEnumerable<Series>> QuerySeriesAsync(string sql, object parameters)
        {
            var seriesDictionary = new Dictionary<string, Series>();
            using var connection = GetConnection();
            var seriesList = await connection.QueryAsync<Series, Genre, Season, Series>(
                sql,
                (series, genre, season) =>
                {
                    if (!seriesDictionary.TryGetValue(series.Id!, out var singleSeries))
                    {
                        singleSeries = new Series
                        {
                            Id = series.Id,
                            Name = series.Name,
                            UserId = series.UserId,
                            ViewingForm = series.ViewingForm,
                            Review = series.Review,
                            StartYear = series.StartYear,
                            EndYear = series.EndYear,
                            SeriesGenres = new(),
                            Seasons = new(),
                        };
                        seriesDictionary.Add(singleSeries!.Id!, singleSeries);
                    }

                    if (!String.IsNullOrEmpty(genre.Name) && singleSeries.SeriesGenres.Find(g => g.Name == genre.Name) is null)
                        singleSeries.SeriesGenres.Add(genre);

                    if (singleSeries.Seasons.Find(s => s.Id == season.Id) is null)
                        singleSeries.Seasons.Add(season);

                    singleSeries.Seasons.OrderBy(se => se.ReleaseYear).ToList();

                    return singleSeries;
                },
                parameters,
                splitOn: "GenreId,SeasonId");

            return seriesList;
        }
    }
}
