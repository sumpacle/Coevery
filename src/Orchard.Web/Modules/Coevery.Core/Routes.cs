﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Mvc.Routes;
using Orchard.ContentManagement.MetaData;

namespace Coevery.Core
{
    public class Routes : IRouteProvider {
        private readonly ShellBlueprint _blueprint;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Routes(ShellBlueprint blueprint, IContentDefinitionManager contentDefinitionManager) {
            _blueprint = blueprint;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {

            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            var displayPathsPerArea = _blueprint.Controllers.GroupBy(
                x => x.AreaName,
                x => x.Feature.Descriptor.Extension);

            yield return new RouteDescriptor {
                Route = new Route(
                    "Coevery",
                    new RouteValueDictionary {
                        {"area", "Coevery.Core"},
                        {"controller", "Home"},
                        {"action", "Index"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "Coevery.Core"}
                    },
                    new MvcRouteHandler())
            };

            foreach (var item in displayPathsPerArea) {
                var areaName = item.Key;
                var extensionDescriptor = item.Distinct().Single();
                var displayPath = extensionDescriptor.Path;
                SessionStateBehavior defaultSessionState;
                Enum.TryParse(extensionDescriptor.SessionState, true /*ignoreCase*/, out defaultSessionState);

                yield return new RouteDescriptor {
                    Priority = -10,
                    SessionState = defaultSessionState,
                    Route = new Route(
                        "Coevery/" + displayPath + "/{controller}/{action}/{id}",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"controller", "home"},
                            {"action", "index"},
                            {"id", ""}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", areaName}
                        },
                        new MvcRouteHandler())
                };
            }

            yield return new RouteDescriptor {
                Priority = -11,
                Route = new CoreRoute()
            };

            //var contentDefinitions = _contentDefinitionManager.ListTypeDefinitions();
            //var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            //foreach (var definition in contentDefinitions) {
            //    var areaName = pluralService.Pluralize(definition.Name);

            //    };
            //}
        }
    }
}