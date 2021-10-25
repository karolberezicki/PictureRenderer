﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using PictureRenderer.Profiles;

namespace PictureRenderer
{
    public static class Picture
    {
        public static string Render(string imagePath, PictureProfileBase profile, LazyLoading lazyLoading)
        {
            return Render(imagePath, profile, string.Empty, lazyLoading);
        }

        public static string Render(string imagePath, PictureProfileBase profile, (double x, double y) focalPoint)
        {
            return Render(imagePath, profile, string.Empty, LazyLoading.Browser, focalPoint);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText, (double x, double y) focalPoint)
        {
            return Render(imagePath, profile, altText, LazyLoading.Browser, focalPoint);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y) focalPoint = default)
        {
            var pictureData = PictureUtils.GetPictureData(imagePath, profile, altText, focalPoint);
            var imgElement = RenderImgElement(pictureData, profile, lazyLoading);
            var sourceElement = RenderSourceElement(pictureData);

            var sourceElementWebp = string.Empty;
            if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
            {
                sourceElementWebp = RenderSourceElement(pictureData, ImageFormat.Webp);
            }

            //Webp source element must be rendered first. Browser selects the first version it supports.
            return $"<picture>{sourceElementWebp}{sourceElement}{imgElement}</picture>";
        }

        private static string RenderImgElement(PictureData pictureData, PictureProfileBase profile, LazyLoading lazyLoading)
        {
            var loadingAttribute = lazyLoading == LazyLoading.Browser ? "loading=\"lazy\"" : string.Empty;
            return $"<img alt=\"{HttpUtility.HtmlEncode(pictureData.AltText)}\" src=\"{pictureData.ImgSrc}\" {loadingAttribute}/>";
        }

        private static string RenderSourceElement(PictureData pictureData, string format = "")
        {
            var srcSet = pictureData.SrcSet;
            var formatAttribute = string.Empty;
            if (format == ImageFormat.Webp)
            {
                srcSet = pictureData.SrcSetWebp;
                formatAttribute = "type=\"image/" + format + "\"";
            }
            var srcSetAttribute = $"srcset=\"{srcSet}\"";
            var sizesAttribute = $"sizes=\"{pictureData.SizesAttribute}\"";

            return $"<source {srcSetAttribute} {sizesAttribute} {formatAttribute}/>";
        }
    }
}
