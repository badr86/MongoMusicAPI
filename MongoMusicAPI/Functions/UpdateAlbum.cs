﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoMusicAPI.Models;
using MongoMusicAPI.Helpers;

namespace MongoMusicAPI.Functions
{
    public class UpdateAlbum
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Album> _albums;

        public UpdateAlbum(
            MongoClient mongoClient,
            ILogger<UpdateAlbum> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase(_config[Settings.DATABASE_NAME]);
            _albums = database.GetCollection<Album>(_config[Settings.COLLECTION_NAME]);
        }

        [FunctionName(nameof(UpdateAlbum))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Album/{id}")] HttpRequest req,
            string id)
        {
            IActionResult returnValue = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var updatedResult = JsonConvert.DeserializeObject<Album>(requestBody);

            updatedResult.Id = id;

            try
            {
                var replacedItem = _albums.ReplaceOne(album => album.Id == id, updatedResult);

                if (replacedItem == null)
                {
                    returnValue = new NotFoundResult();
                }
                else
                {
                    returnValue = new OkObjectResult(updatedResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not update Album with id: {id}. Exception thrown: {ex.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }
    }
}