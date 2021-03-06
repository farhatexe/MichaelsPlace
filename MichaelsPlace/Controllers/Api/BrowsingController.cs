﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Routing.Constraints;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MichaelsPlace.Infrastructure;
using MichaelsPlace.Models;
using MichaelsPlace.Models.Api;
using MichaelsPlace.Models.Persistence;
using MichaelsPlace.Queries;

namespace MichaelsPlace.Controllers.Api
{
    [RoutePrefix("browsing")]
    public class BrowsingController : SpaApiControllerBase
    {
        private readonly IQueryFactory _queryFactory;

        public BrowsingController(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        /// <summary>
        /// Returns the article with <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("articles/{id:int}")]
        public BrowsingItemModel ArticleById(int id) => _queryFactory.Create<ByIdQuery<Article>>().Execute<BrowsingItemModel>(id);

        /// <summary>
        /// Returns the to-do with <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("todos/{id:int}")]
        public BrowsingItemModel ToDoById(int id) => _queryFactory.Create<ByIdQuery<ToDo>>().Execute<BrowsingItemModel>(id);


        /// <summary>
        /// Returns all articles which match the provided <paramref name="situation"/>.
        /// The situation should be provided in serialized form, like 1.2-3-4.5.6.
        /// </summary>
        /// <param name="situation">The situation should be provided in serialized form, like 1.2-3-4.5.6.</param>
        /// <returns></returns>
        [HttpGet, Route("articles/{situation:situation}")]
        public List<BrowsingItemModel> ArticleBySituation(SituationModel situation) =>
            _queryFactory.Create<ItemBySituationQuery>()
                         .Execute<Article>(situation)
                         .ProjectTo<BrowsingItemModel>()
                         .ToList();

        /// <summary>
        /// Returns all to-dos which match the provided <paramref name="situation"/>.
        /// The situation should be provided in serialized form, like 1.2-3-4.5.6.
        /// </summary>
        /// <param name="situation">The situation should be provided in serialized form, like 1.2-3-4.5.6.</param>
        /// <returns></returns>
        [HttpGet, Route("todos/{situation:situation}")]
        public List<BrowsingItemModel> ToDoBySituation(SituationModel situation) =>
            _queryFactory.Create<ItemBySituationQuery>()
                         .Execute<ToDo>(situation)
                         .ProjectTo<BrowsingItemModel>()
                         .ToList();
    }

    public class SituationConstraint : RegexRouteConstraint, IHttpRouteConstraint
    {
        public SituationConstraint() : base(@"(\d+\.?)-(\d+\.?)-(\d+\.?)")
        {
        }
    }
}
