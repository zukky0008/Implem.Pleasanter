﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Images;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTemplates
    {
        public static HtmlBuilder Template(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            long siteId = 0,
            long parentId = 0,
            string referenceType = null,
            string siteReferenceType = null,
            string title = null,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            string script = null,
            string userScript = null,
            string userStyle = null,
            Action action = null)
        {
            return hb.Container(context: context, action: () => hb
                .MainContainer(
                    context: context,
                    ss: ss,
                    view: view,
                    verType: verType,
                    methodType: methodType,
                    siteId: siteId,
                    parentId: parentId,
                    referenceType: referenceType,
                    siteReferenceType: siteReferenceType,
                    title: title,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    useSearch: useSearch,
                    useNavigationMenu: useNavigationMenu,
                    action: action)
                .HiddenData(context: context)
                .VideoDialog(context: context, ss: ss)
                .Styles(context: context, ss: ss, userStyle: userStyle)
                .Scripts(context: context, ss: ss, script: script, userScript: userScript));
        }

        private static HtmlBuilder Container(this HtmlBuilder hb, Context context, Action action)
        {
            if (!context.Ajax)
            {
                return hb.Html(action: () => hb
                    .Head(action: () => hb
                        .Meta(httpEquiv: "X-UA-Compatible", content: "IE=edge")
                        .Meta(httpEquiv: "content-language", content: context.Language)
                        .Meta(charset: "utf-8")
                        .Meta(name: "keywords", content: Parameters.General.HtmlHeadKeywords)
                        .Meta(name: "description", content: Parameters.General.HtmlHeadDescription)
                        .Meta(name: "author", content: Parameters.General.HtmlHeadAuther)
                        .Meta(name: "viewport", content: Parameters.General.HtmlHeadViewport)
                        .LinkedStyles(context: context)
                        .Title(action: () => hb
                            .Text(text: Parameters.General.HtmlTitle
                                ?? Displays.ProductName(context: context))))
                    .Body(style: "visibility:hidden", action: action));
            }
            else
            {
                action?.Invoke();
                return hb;
            }
        }

        public static HtmlBuilder MainContainer(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            long siteId,
            long parentId,
            string referenceType,
            string siteReferenceType,
            string title,
            Error.Types errorType = General.Error.Types.None,
            string[] messageData = null,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            Action action = null)
        {
            return hb.Div(id: "MainContainer", action: () => hb
                .Header(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    referenceType: referenceType,
                    errorType: errorType,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch)
                .Content(
                    context: context,
                    ss: ss,
                    view: view,
                    errorType: errorType,
                    messageData: messageData,
                    siteId: siteId,
                    title: title,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    action: action)
                .Div(id: "BottomMargin both")
                .Footer()
                .BackUrl(
                    context: context,
                    siteId: siteId,
                    parentId: parentId,
                    referenceType: referenceType,
                    siteReferenceType: siteReferenceType));
        }

        private static HtmlBuilder Content(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Error.Types errorType,
            string[] messageData,
            long siteId = 0,
            string title = null,
            bool useBreadcrumb = true,
            bool useTitle = true,
            Action action = null)
        {
            return hb.Article(id: "Application", action: () =>
            {
                if (!errorType.Has())
                {
                    hb.Nav(css: "both cf", action: () => hb
                        .Breadcrumb(
                            context: context,
                            ss: ss,
                            view: view,
                            _using: useBreadcrumb));
                    if (useTitle)
                    {
                        hb.Title(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            text: title);
                    }
                    action?.Invoke();
                    hb.Message(message: context.Message());
                }
                else
                {
                    var message = errorType.Message(
                        context: context,
                        data: messageData);
                    hb
                        .Message(message: message)
                        .MainCommands(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            verType: Versions.VerTypes.Latest);
                }
            });
        }

        private static HtmlBuilder Message(this HtmlBuilder hb, Message message)
        {
            return hb
                .P(id: "Message", css: "message")
                .Hidden(
                    controlId: "MessageData",
                    value: message?.ToJson(),
                    _using: message != null);
        }

        private static HtmlBuilder Title(
            this HtmlBuilder hb, Context context, SiteSettings ss, long siteId, string text)
        {
            return !text.IsNullOrEmpty()
                ? hb
                    .Div(id: "SiteImageIconContainer", action: () => hb
                        .SiteImageIcon(context: context, ss: ss, siteId: siteId))
                    .Header(id: "HeaderTitleContainer", action: () => hb
                        .H(number: 1, id: "HeaderTitle", action: () => hb
                            .Text(text: text)))
                : hb;
        }

        public static HtmlBuilder SiteImageIcon(
            this HtmlBuilder hb, Context context, SiteSettings ss, long siteId)
        {
            return BinaryUtilities.ExistsSiteImage(
                context: context,
                ss: ss,
                referenceId: siteId,
                sizeType: ImageData.SizeTypes.Icon)
                    ? hb.Img(
                        src: Locations.Get(
                            context: context,
                            parts: new string[]
                            {
                                "Items",
                                siteId.ToString(),
                                "Binaries",
                                "SiteImageIcon",
                                BinaryUtilities.SiteImagePrefix(
                                    context: context,
                                    ss: ss,
                                    referenceId: siteId,
                                    sizeType: ImageData.SizeTypes.Icon)
                            }),
                        css: "site-image-icon")
                    : hb;
        }

        private static HtmlBuilder VideoDialog(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return context.ContractSettings.Attachments() != false && !context.Mobile
                ? hb
                    .Div(
                        attributes: new HtmlAttributes()
                            .Id("VideoDialog")
                            .Class("dialog")
                            .Title(Displays.Camera(context: context)),
                        action: () => hb
                            .Div(action: () => hb
                                .Video(id: "Video"))
                            .Hidden(controlId: "VideoTarget")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.ToShoot(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.toShoot($(this));",
                                    icon: "ui-icon-video")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel")))
                    .Canvas(id: "Canvas")
                : hb;
        }

        private static HtmlBuilder HiddenData(this HtmlBuilder hb, Context context)
        {
            return !context.Ajax
                ? hb
                    .Hidden(controlId: "ApplicationPath", value: Locations.Get(context: context))
                    .Hidden(controlId: "Language", value: context.Language)
                    .Hidden(controlId: "DeptId", value: context.DeptId.ToString())
                    .Hidden(controlId: "UserId", value: context.UserId.ToString())
                : hb;
        }

        public static string Error(
            Context context, Error.Types errorType, string[] messageData = null)
        {
            var hb = new HtmlBuilder();
            var ss = new SiteSettings();
            return hb.Container(context: context, action: () => hb
                .MainContainer(
                    context: context,
                    ss: ss,
                    view: null,
                    verType: Versions.VerTypes.Latest,
                    methodType: BaseModel.MethodTypes.NotSet,
                    messageData: messageData,
                    siteId: 0,
                    parentId: 0,
                    referenceType: null,
                    siteReferenceType: null,
                    title: null,
                    errorType: errorType,
                    useBreadcrumb: false,
                    useTitle: false,
                    useNavigationMenu: true)
                .HiddenData(context: context)
                .Styles(context: context, ss: ss)
                .Scripts(context: context, ss: ss))
                    .ToString();
        }
    }
}