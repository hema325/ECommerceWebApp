
using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ITokenRepository Tokens { get; }
        IUserRoleRepository UserRoles { get; }
        IMessageRepository Messages { get; }
        IConversationRepository Conversations { get; }
        IUserConversationRepository UserConversations { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IItemRepository Items { get; }
        IVariationRepository Variations { get; }
        IOptionRepository Options { get; }
        IItemOptionRepository ItemOptions { get; }
        IDiscountRepository Discounts { get; }
        IProductDiscountRepository ProductDiscounts { get; }
        ICartItemRepository CartItems { get; }
        ICountryRepository Countries { get; }
        IAddressRepository Addresses { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IReviewRepository Reviews { get; }
        IUserLoginRepository UserLogins { get; set; }

    }
}
