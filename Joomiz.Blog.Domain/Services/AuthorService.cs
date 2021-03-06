﻿using Joomiz.Blog.Domain.Common;
using Joomiz.Blog.Domain.Contracts.Repositories;
using Joomiz.Blog.Domain.Contracts.Services;
using Joomiz.Blog.Domain.Contracts.Validation;
using Joomiz.Blog.Domain.Model;
using System;

namespace Joomiz.Blog.Domain.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IAuthorValidation authorValidation;

        public AuthorService(IAuthorRepository authorRepository, IAuthorValidation authorValidation)
        {
            this.authorRepository = authorRepository;
            this.authorValidation = authorValidation;
        }

        public Author GetById(int id)
        {
            return this.authorRepository.GetById(id);
        }

        public Author GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return this.authorRepository.GetByName(name);
        }

        public Author GetByNameByPassword(string name, string password)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            Author author = this.authorRepository.GetByName(name);

            if (author != null && author.Password == password)
            {
                return author;
            }
            else
            {
                return null;
            }

        }

        public PagedList<Author> GetAll(int pageNumber = 1, int pageSize = 50)
        {
            return this.authorRepository.GetAll(pageNumber, pageSize);
        }

        public IValidationResult Add(Author obj)
        {
            if (obj == null)
                throw new NullReferenceException("obj");

            obj.DateCreated = DateTime.UtcNow;

            var validationResult = this.authorValidation.Validate(obj);

            if (validationResult.IsValid)
                this.authorRepository.Add(obj);

            return validationResult;
        }

        public IValidationResult ChangePassword(string name, string password, string newPassword)
        {
            Author author = this.GetByName(name);

            if (author == null)
                throw new Exception("Not found an author with provided information.");

            if(author.Password != password)
                throw new Exception("Not found an author with provided information.");

            author.Password = newPassword;

            var validationResult = this.authorValidation.Validate(author);

            if(validationResult.IsValid)
                this.authorRepository.Update(author);

            return validationResult;
        }

        public IValidationResult Update(Author obj)
        {
            if (obj == null)
                throw new NullReferenceException("obj");

            var author = this.GetById(obj.Id);

            if (author == null)
                throw new Exception(string.Format("Author {0} not found.", obj.Id));

            author.IsActive = obj.IsActive;
            author.Name = obj.Name;
            author.Email = obj.Email;

            var validationResult = this.authorValidation.Validate(author);

            if (validationResult.IsValid)                
                this.authorRepository.Update(author);

            return validationResult;
        }

        public void Delete(int id)
        {
            this.authorRepository.Delete(id);
        }
    }
}
