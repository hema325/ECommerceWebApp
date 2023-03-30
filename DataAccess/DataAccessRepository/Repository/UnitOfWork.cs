using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        #region props
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public ITokenRepository Tokens { get; }
        public IUserRoleRepository UserRoles { get; }
        public IMessageRepository Messages { get; }
        public IConversationRepository Conversations { get; }
        public IUserConversationRepository UserConversations { get; }
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public IItemRepository Items { get; }
        public IVariationRepository Variations { get; }
        public IOptionRepository Options { get; }
        public IItemOptionRepository ItemOptions { get; }
        public IDiscountRepository Discounts { get; }
        public IProductDiscountRepository ProductDiscounts { get; }
        public ICartItemRepository CartItems { get; }
        public ICountryRepository Countries { get; }
        public IAddressRepository Addresses { get; set; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IReviewRepository Reviews { get; }
        public IUserLoginRepository UserLogins { get; set; }

        #endregion

        #region cons
        public UnitOfWork(string connectionString)
        {
            Users = new UserRepository(connectionString);
            Roles = new RoleRepository(connectionString);
            Tokens = new TokenRepository(connectionString);
            UserRoles = new UserRoleRepository(connectionString);
            Messages = new MessageRepository(connectionString);
            Conversations = new ConversationRepository(connectionString);
            UserConversations = new UserConversationRepository(connectionString);
            Categories = new CategoryRepository(connectionString);
            Products = new ProductRepository(connectionString);
            Items = new ItemRepository(connectionString);
            Variations = new VariationRepository(connectionString);
            Options = new OptionRepository(connectionString);
            ItemOptions = new ItemOptionRepository(connectionString);
            Discounts = new DiscountRepository(connectionString);
            ProductDiscounts = new ProductDiscountRepository(connectionString);
            CartItems = new CartItemRepository(connectionString);
            Countries = new CountryRepository(connectionString);
            Addresses = new AddressRepository(connectionString);
            Orders = new OrderRepository(connectionString);
            OrderItems = new OrderItemRepository(connectionString);
            Reviews = new ReviewRepository(connectionString);
            UserLogins = new UserLoginRepository(connectionString);
        }
        #endregion

    }
}
