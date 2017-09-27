using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace place
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Content/Scripts/jquery.unobtrusive*",
                "~/Content/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Content/Scripts/knockout-{version}.js",
                "~/Content/Scripts/knockout.validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Content/Scripts/sammy-{version}.js",
                "~/Content/Scripts/app/common.js",
                "~/Content/Scripts/app/app.datamodel.js",
                "~/Content/Scripts/app/app.viewmodel.js",
                "~/Content/Scripts/app/home.viewmodel.js",
                "~/Content/Scripts/app/_run.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Content/Scripts/bootstrap.js",
                "~/Content/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                 "~/Content/Css/bootstrap.css"));
        }
    }
}
