using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myProject.Abstractions;
using myProject.Data;
using myProject.Abstractions.Data.Repositories;
using myProject.Data.Entities;
using Microsoft.EntityFrameworkCore;
using myProject.Repositories.Implementations;

namespace myProject.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyProjectContext _dbContext;

        private readonly IArticleRepository _articleRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<NewsResource> _newsResourceRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Subscription> _subscriptionRepository;
        private readonly IRepository<UserCategory> _userCategoryRepository;
        private readonly IRepository<User> _userRepository;


        public UnitOfWork(MyProjectContext dbContext,
            IArticleRepository articleRepository,
            IRepository<Category> categoryRepository,
            IRepository<Comment> commentRepository,
            IRepository<NewsResource> newsResourceRepository,
            IRepository<Role> roleRepository,
            IRepository<Subscription> subscriptionRepository,
            IRepository<UserCategory> userCategoryRepository,
            IRepository<User> userRepository)
        {
            _dbContext = dbContext;
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _newsResourceRepository = newsResourceRepository;
            _roleRepository = roleRepository;
            _subscriptionRepository = subscriptionRepository;
            _userCategoryRepository = userCategoryRepository;
            _userRepository = userRepository;
        }

        public IArticleRepository Articles => _articleRepository;
        public IRepository<Category> Categories => _categoryRepository;
        public IRepository<Comment> Comments => _commentRepository;
        public IRepository<NewsResource> NewsResources => _newsResourceRepository;
        public IRepository<Role> Roles => _roleRepository;
        public IRepository<Subscription> Subscriptions => _subscriptionRepository;
        public IRepository<UserCategory> UserCategories => _userCategoryRepository;
        public IRepository<User> Users => _userRepository;

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _articleRepository?.Dispose();
            _categoryRepository?.Dispose();
            _commentRepository?.Dispose();
            _newsResourceRepository?.Dispose();
            _roleRepository?.Dispose();
            _subscriptionRepository?.Dispose();
            _userCategoryRepository?.Dispose();
            _userRepository?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
