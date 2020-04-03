﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTasks.Services
{
    public class ImageService
    {
        const int DownloadImageTimeoutSeconds = 15;

        readonly HttpClient HttpClient_ = new HttpClient {
            Timeout = TimeSpan.FromSeconds(DownloadImageTimeoutSeconds)
        };

        async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                using (HttpResponseMessage httpResponse = await HttpClient_.GetAsync(imageUrl))
                {
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return await httpResponse.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> DownloadImageAsBase64Async(string imageUrl)
        {
            byte[] imageByteArray = await DownloadImageAsync(imageUrl);
            return System.Convert.ToBase64String(imageByteArray);
        }

        public ImageSource ConvertImageFromBase64ToImageSource(string imageBase64)
        {
            if (!string.IsNullOrEmpty(imageBase64))
            {
                return ImageSource.FromStream(() =>
                    new MemoryStream(System.Convert.FromBase64String(imageBase64))
                );
            }
            else
            {
                return null;
            }
        }

    }
}
