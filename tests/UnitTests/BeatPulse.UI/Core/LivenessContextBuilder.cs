using BeatPulse.UI.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.BeatPulse.UI.Core
{
    class LivenessContextBuilder
    {
        private List<LivenessConfiguration> _configurations;
        private string _databaseName;

        public LivenessContextBuilder()
        {
            _configurations = new List<LivenessConfiguration>();
            _databaseName = "testdb";
        }
        public LivenessContextBuilder WithLiveness(LivenessConfiguration liveness)
        {
            if (liveness != null)
            {
                _configurations.Add(liveness);
            }

            return this;
        }

        public LivenessContextBuilder WithDatabaseName(string databaseName)
        {
            _databaseName = databaseName ?? "testdb";

            return this;
        }


        public LivenessContextBuilder WithRandomDatabaseName()
        {
            _databaseName = Guid.NewGuid().ToString();

            return this;
        }

        public LivenessDb Build()
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder();
            contextOptionsBuilder.UseSqlite($"Data Source={_databaseName}");

            var context = new LivenessDb(contextOptionsBuilder.Options);
            context.Database.Migrate();

            if (_configurations.Any())
            {
                context.LivenessConfiguration.AddRange(_configurations);
                context.SaveChanges();
            }

            return context;
        }
    }
}
