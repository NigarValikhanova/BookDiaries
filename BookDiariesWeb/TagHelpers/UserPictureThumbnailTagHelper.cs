﻿using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BookDiariesWeb.TagHelpers
{
    public class UserPictureThumbnailTagHelper : TagHelper
    {
        public string? PictureUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            if (string.IsNullOrEmpty(PictureUrl))
            {
                output.Attributes.SetAttribute("src", "/userPictures/default.jpg");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/userPictures/{PictureUrl}");
            }

        }
    }
}

