﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public class TestCategoryVM : NotifyPropertyChangedHandler
    {
        public TestCategory Model { get; set; }

        public int Id { get => Model.Id; }

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }


        public TestCategoryVM(TestCategory testCategory)
        {
            Model = testCategory;
        }


        public override bool Equals(object? obj)
        {
            if (obj != null && obj is TestCategoryVM testCategoryVM && testCategoryVM.Model != null)
            {
                return Model.Id.Equals(testCategoryVM.Model.Id);
            }
            return false;
        }
    }
}
