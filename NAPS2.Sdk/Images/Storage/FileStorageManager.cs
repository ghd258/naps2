﻿using System;
using System.Globalization;
using System.IO;

namespace NAPS2.Images.Storage
{
    public class FileStorageManager : IDisposable
    {
        private readonly string _prefix = Path.GetRandomFileName();
        private int _fileNumber;
        
        public FileStorageManager() : this(Paths.Temp)
        {
        }

        public FileStorageManager(string folderPath)
        {
            FolderPath = folderPath;
        }

        protected string FolderPath { get; }
        
        public virtual string NextFilePath()
        {
            lock (this)
            {
                string fileName = $"{_prefix}.{(++_fileNumber).ToString("D5", CultureInfo.InvariantCulture)}";
                return Path.Combine(FolderPath, fileName);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
