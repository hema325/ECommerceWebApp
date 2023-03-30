using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class ReviewRepository:EntityRepository<Review>,IReviewRepository
    {
        public ReviewRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Review.User));
        }

        public async Task<IEnumerable<Review>> GetByItemIdAsync(int itemId, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@"select Rs.Id,Rs.Comment,Us.Id,Us.FirstName,Us.LastName,Us.ImgUrl from Reviews as Rs
                          join Users as Us on Us.Id = Rs.UserId
                          where Rs.ItemId = @itemId");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by Rs.Id")
                    .Append(" ")
                    .Append($"Offset @skip Rows");

                if (take != null)
                {
                    sql.Append(" ")
                        .Append($"Fetch next @take Rows Only");
                }
            }
            

            return await QueryAsync<Review, User, Review>(sql.ToString(), (review, user) =>
            {
                review.User = user;
                return review;
            }, new { itemId = itemId,skip = skip, take = take });
        }

    }
}
