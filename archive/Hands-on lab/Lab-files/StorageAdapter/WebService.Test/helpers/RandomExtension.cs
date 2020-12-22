// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text;

namespace WebService.Test.helpers
{
    public static class RandomExtension
    {
        public static string NextString(this Random rand, int length = 32, string characters = @"0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            var builder = new StringBuilder();

            while (builder.Length < length)
            {
                builder.Append(characters[rand.Next(0, characters.Length)]);
            }

            return builder.ToString();
        }

        public static DateTimeOffset NextDateTimeOffset(this Random rand)
        {
            var min = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var max = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);

            return min + TimeSpan.FromSeconds(rand.Next(0, (int)(max - min).TotalSeconds));
        }
    }
}
