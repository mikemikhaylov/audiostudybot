using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.Domain.Model;
using MongoDB.Driver;

namespace AudioStudy.Bot.DataAccess.Db
{
    public class MongoDbUserRepository : MongoDbRepositoryBase<User>, IUserRepository
    {
        public MongoDbUserRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
        {
        }

        public async Task<User> GetUserByChatIdAsync(long chatId)
        {
            return await MongoDbContext.Users
                .Find(x => x.ChatId == chatId).SingleOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            await MongoDbContext.Users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(User user, UserUpdateCommand command)
        {
            UpdateDefinition<User> update = null;
            var updateActions = new List<Action<User>>();
            if (command.State != null)
            {
                updateActions.Add(u => u.State = command.State.Value);
                update = GetUpdateDef(update, u => u.State, command.State.Value);
            }

            if (command.Language != null)
            {
                updateActions.Add(u => u.Language = command.Language.Value);
                update = GetUpdateDef(update, u => u.Language, command.Language.Value);
            }
            
            if (command.LearningLanguage != null)
            {
                updateActions.Add(u => u.LearningLanguage = command.LearningLanguage.Value);
                update = GetUpdateDef(update, u => u.LearningLanguage, command.LearningLanguage.Value);
            }
            
            if (command.KnowsLanguage != null)
            {
                updateActions.Add(u => u.KnowsLanguage = command.KnowsLanguage.Value);
                update = GetUpdateDef(update, u => u.KnowsLanguage, command.KnowsLanguage.Value);
            }

            if (command.RatingDate != null)
            {
                updateActions.Add(u => u.RatingDate = command.RatingDate.Value);
                update = GetUpdateDef(update, u => u.RatingDate, command.RatingDate.Value);
            }
            
            if (command.UserCourses != null)
            {
                updateActions.Add(u => u.Courses = command.UserCourses.Value);
                update = GetUpdateDef(update, u => u.Courses, command.UserCourses.Value);
            }

            if (update != null)
            {
                var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
                await MongoDbContext.Users.UpdateOneAsync(filter, update);
                updateActions.ForEach(x => x(user));
            }
        }

        private UpdateDefinition<User> GetUpdateDef<TField>(UpdateDefinition<User> update,
            Expression<Func<User, TField>> field, TField value)
        {
            return update == null ? Builders<User>.Update.Set(field, value) : update.Set(field, value);
        }

        protected override void InitAction(MongoDbContext mongoDbContext)
        {
            var options = new CreateIndexOptions<User>
            {
                Unique = true
            };
            var indexKeysDefinition = Builders<User>.IndexKeys.Ascending(x => x.ChatId);
            mongoDbContext.Users.Indexes.CreateOne(new CreateIndexModel<User>(indexKeysDefinition, options));
        }
    }
}