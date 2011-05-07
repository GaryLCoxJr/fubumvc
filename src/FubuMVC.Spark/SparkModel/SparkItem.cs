﻿using System;
using System.Collections.Generic;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkDescriptor
    {
        string Name { get; } 
    }

    public class ViewDescriptor
    {
        public string Name { get { return "View"; } }
        public ITemplate Master { get; set; }
        public Type ViewModel { get; set; }
        public string Namespace { get; set; }
    }

    public class NulloDescriptor : ISparkDescriptor
    {
        public string Name { get { return "Undefined"; } }
    }

    public interface ITemplate
    {
        string FilePath { get; }
        string RootPath { get; }
        string Origin { get; }
        string ViewPath { get; set; }
    }

    public class Template : ITemplate
    {
        public Template()
        {
            Descriptor = new NulloDescriptor();
        }

        public string FilePath { get; set; }
        public string RootPath { get; set; }
        public string Origin { get; set; }
        public string ViewPath { get; set; }

        public ISparkDescriptor Descriptor { get; set; }
    }

    public class SparkItem : ITemplate
    {
        public SparkItem(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }
        public string ViewPath { get; set; }

        public SparkItem Master { get; set; }
        public Type ViewModelType { get; set; }        
        public string Namespace { get; set; }

	    public bool HasViewModel()
	    {
	        return ViewModelType != null;
	    }

	    public override string ToString()
        {
            return FilePath;
        }
    }

    // TODO : This is a bit silly. Rework pending. 

    public class SparkItems : List<SparkItem>, ISparkItems
    {
        public SparkItems() {}
        public SparkItems(IEnumerable<SparkItem> items) : base(items) {}
    }

    public interface ISparkItems : IEnumerable<SparkItem> {}
}