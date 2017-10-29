// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();

        #region Samples

        [Category("Restriction Operators")]
        [Title("Where - Sample 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;

            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums)
            {
                Console.WriteLine(x);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Sample 2")]
        [Description("This sample return return all presented in market products")]

        public void Linq2()
        {
            var products =
                from p in dataSource.Products
                where p.UnitsInStock > 0
                select p;

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }

        #endregion

        #region Task1

        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("This sample return return all clients with sum of orders cost higher than some value")]

        public void Linq3()
        {
            List<decimal> values = new List<decimal>(new decimal[] {1000, 2000, 5000, 10000, 17000, 30000});

            foreach (var value in values)
            {
                ObjectDumper.Write($"Clients with total costs higher than {value}");
                var clients = dataSource.Customers.Where(n => n.Orders.Select(m => m.Total).Sum() > value);
                foreach (var p in clients)
                {
                    ObjectDumper.Write(p);
                }
            }
        }

        #endregion

        #region Task2

        [Category("Restriction Operators")]
        [Title("Where - Task 2 (Without group by)")]
        [Description("This sample return return all suppliers in the same city with client for each customer")]

        public void Linq4()
        {
            var suppliers = dataSource.Customers.Select(n => new
            {
                Suppliers = dataSource.Suppliers.Where(s => s.Country == n.Country && s.City == n.City),
                Customer = n
            });

            foreach (var p in suppliers)
            {
                ObjectDumper.Write("--------------");
                ObjectDumper.Write($"For Customer {p.Customer.CompanyName}");
                if (!p.Suppliers.Any())
                {
                    ObjectDumper.Write("no suppliers in the same city");
                }
                else
                {
                    ObjectDumper.Write(p.Suppliers);
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2 (With group by)")]
        [Description("This sample return return all clients with sum of orders cost higher than some value")]

        public void Linq5()
        {
            var customers = dataSource.Customers.GroupBy(n => new { n.City, n.Country });
            foreach (var customer in customers)
            {
                ObjectDumper.Write("--------------");
                ObjectDumper.Write($"For Customers in  {customer.Key}");
                ObjectDumper.Write(dataSource.Suppliers.Where(n => n.City == customer.Key.City && n.Country == customer.Key.Country));
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2 (With group by and single query)")]
        [Description("This sample return return all clients with sum of orders cost higher than some value")]

        public void Linq05()
        {
            var customers = dataSource.Customers.GroupBy(n => new { n.City, n.Country });
            foreach (var customer in customers)
            {
                ObjectDumper.Write("--------------");
                ObjectDumper.Write($"For Customers in  {customer.Key}");
                ObjectDumper.Write(dataSource.Suppliers.Where(n => n.City == customer.Key.City && n.Country == customer.Key.Country));
            }
        }

        #endregion

        #region Task3

        [Title("Where - Task 3")]
        [Description("This sample return return all clients with any orders cost higher than some value")]

        public void Linq6()
        {
            List<decimal> values = new List<decimal>(new decimal[] { 10, 200, 500, 1000, 2500, 10000 });

            foreach (var value in values)
            {
                ObjectDumper.Write($"Clients with Any order cost higher than {value}");
                var clients = dataSource.Customers.Where(n => n.Orders.Any(q => q.Total > value));
                foreach (var p in clients)
                {
                    ObjectDumper.Write(p);
                }
            }
        }

        #endregion

        #region Task4

        [Category("Restriction Operators")]
        [Title("Where - Task 4")]
        [Description("This sample return return all clients with where start ordering date")]

        public void Linq7()
        {
            foreach (var customer in dataSource.Customers)
            {
                var startdate = customer.Orders.OrderBy(n => n.OrderDate).FirstOrDefault()?.OrderDate;
                if (startdate != null)
                {
                    ObjectDumper.Write($"Customer {customer.CompanyName} start ordering {startdate}");
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 4 (Single query)")]
        [Description("This sample return return all clients with where start ordering date")]

        public void Linq07()
        {
            var customers = dataSource.Customers.Select(q => new { q.CompanyName, q.Orders.OrderBy(n => n.OrderDate).FirstOrDefault()?.OrderDate });
            foreach (var customer in customers)
            {
                if (customer.OrderDate != null)
                {
                    ObjectDumper.Write($"Customer {customer.CompanyName} start ordering {customer.OrderDate}");
                }
            }
        }

        #endregion

        #region Task5

        [Category("Restriction Operators")]
        [Title("Where - Task 5")]
        [Description("This sample return return all clients with where start ordering date with sorting")]

        public void Linq8()
        {
            var customers = dataSource.Customers.Select(n => new {Customer = n, StartDate = n.Orders.OrderBy(q => q.OrderDate).FirstOrDefault()?.OrderDate});
            ObjectDumper.Write("Ordered by date : ");
            foreach (var customer in customers.OrderBy(n => n.StartDate))
            {
                ObjectDumper.Write($"Customer {customer.Customer.CompanyName} start ordering {customer.StartDate}");
            }
            ObjectDumper.Write("-----------------------------");
            ObjectDumper.Write("Ordered by total : ");
            foreach (var customer in customers.OrderBy(n => n.Customer.Orders.Select(q => q.Total).Sum()))
            {
                ObjectDumper.Write($"Customer {customer.Customer.CompanyName} start ordering {customer.StartDate}");
            }
            ObjectDumper.Write("-----------------------------");
            ObjectDumper.Write("Ordered by name : ");
            foreach (var customer in customers.OrderBy(n => n.Customer.CompanyName))
            {
                ObjectDumper.Write($"Customer {customer.Customer.CompanyName} start ordering {customer.StartDate}");
            }
            ObjectDumper.Write("-----------------------------");

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 5 (Single quuery)")]
        [Description("This sample return return all clients with where start ordering date with sorting")]

        public void Linq08()
        {
            var customers = dataSource.Customers.Select(n => new { Customer = n, StartDate = n.Orders.OrderBy(q => q.OrderDate).FirstOrDefault()?.OrderDate });
            ObjectDumper.Write("Ordered by date : ");
            foreach (var customer in customers.OrderBy(n => n.StartDate).ThenBy(n => n.Customer.Orders.Select(q => q.Total).Sum()).ThenBy(n => n.Customer.CompanyName))
            {
                ObjectDumper.Write($"Customer {customer.Customer.CompanyName} start ordering {customer.StartDate}");
            }
        }

        #endregion

        #region Task6

        [Category("Restriction Operators")]
        [Title("Where - Task 6")]
        [Description("This sample return return all clients without normal codes")]

        public void Linq9()
        {
            var customers = dataSource.Customers.Where(n => !IsCodeValid(n.PostalCode) || !IsCodeValid(n.Phone) || !IsCodeValid(n.Region));
            foreach (var p in customers)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 6 (Other conditions)")]
        [Description("This sample return return all clients without normal codes")]

        public void Linq09()
        {
            var customers = dataSource.Customers.Where(n => (n.PostalCode != null && !n.PostalCode.All(char.IsDigit)) || string.IsNullOrEmpty(n.Phone) || !IsCodeValid(n.Region));
            foreach (var p in customers)
            {
                ObjectDumper.Write(p);
            }
        }

        private bool IsCodeValid(string s)
        {
            return s != null && (s.Contains(')') || s.Contains('('));
        }

        #endregion

        #region Task7

        [Category("Restriction Operators")]
        [Title("Where - Task 7")]
        [Description("Grouping products")]

        public void Linq10()
        {
            var products = dataSource.Products.GroupBy(n => n.Category);
            foreach (var group in products)
            {
                ObjectDumper.Write(group.Key);
                foreach (var miniGroup in group.GroupBy(n => n.UnitsInStock))
                {
                    ObjectDumper.Write($"---{miniGroup.Key}");
                    foreach (var product in miniGroup.OrderBy(n => n.UnitPrice))
                    {
                        ObjectDumper.Write($"--------{product.ProductName}");
                    }
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 7 (Single query)")]
        [Description("Grouping products")]

        public void Linq010()
        {
            var products = dataSource.Products.GroupBy(n => n.Category).Select(n => n.GroupBy(q => q.UnitsInStock).Select(w => w.OrderBy(e => e.UnitPrice)).SelectMany(t => t)).SelectMany(r => r);
            foreach (var product in products)
            {
                ObjectDumper.Write($"{product.ProductName}");
            }
        }

        #endregion

        #region Task8

        [Category("Restriction Operators")]
        [Title("Where - Task 8")]
        [Description("Grouping products by cost")]

        public void Linq11()
        {
            var products = dataSource.Products.GroupBy(n => GetCostType(n));
            foreach (var group in products)
            {
                ObjectDumper.Write($"{group.Key}");
                foreach (var product in group)
                {
                    ObjectDumper.Write($"---{product.ProductName}");
                }
            }
        }

        public enum ProductsCostType
        {
            Cheap,
            Medium,
            Expensive
        }

        private ProductsCostType GetCostType(Product product, decimal lowBorder = 20, decimal highBorder = 60)
        {
            if (product == null)
            {
                throw new ArgumentNullException($"{nameof(product)} cannot be null");
            }
            if (product.UnitPrice < lowBorder)
            {
                return ProductsCostType.Cheap;
            }
            if (product.UnitPrice > highBorder)
            {
                return ProductsCostType.Expensive;
            }
            return ProductsCostType.Medium;
        }

        #endregion

        #region Task9

        [Category("Restriction Operators")]
        [Title("Where - Task 9")]
        [Description("Information about cities")]

        public void Linq12()
        {
            var customersByCities = dataSource.Customers.GroupBy(n => new {n.City, n.Country});
            foreach (var customerGroup in customersByCities)
            {
                ObjectDumper.Write(customerGroup.Key);
                var allOrders = customerGroup.SelectMany(n => n.Orders).Select(n => n.Total);
                var amountOfOrders = allOrders.Count() / customerGroup.Count();
                ObjectDumper.Write($"Average order price: {allOrders.Average()}; Average amount of orders on customer: {amountOfOrders}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 9 (Single query)")]
        [Description("Information about cities (Single query)")]

        public void Linq012()
        {
            var customersByCities = dataSource.Customers.GroupBy(n => new { n.City, n.Country }).Select(n => new
            {
                Place = n.Key,
                Price = n.SelectMany(e => e.Orders).Select(e => e.Total).Average(),
                Amount = n.SelectMany(e => e.Orders).Select(e => e.Total).Count() / n.Count()
            });
            foreach (var customerGroup in customersByCities)
            {
                ObjectDumper.Write(customerGroup.Place);
                ObjectDumper.Write($"Average order price: {customerGroup.Price}; Average amount of orders on customer: {customerGroup.Amount}");
            }
        }

        #endregion

        #region Task10

        [Category("Restriction Operators")]
        [Title("Where - Task 10")]
        [Description("Information by time")]

        public void Linq13()
        {
            var allOrders = dataSource.Customers.SelectMany(n => n.Orders);
            ObjectDumper.Write("By month");
            var monthGroup = allOrders.GroupBy(n => n.OrderDate.Month);
            foreach (var group in monthGroup.OrderBy(n => n.Key))
            {
                ObjectDumper.Write("-----------------------------------");
                ObjectDumper.Write(group.Key);
                ObjectDumper.Write($"Number of orders: {group.Count()}");
            }
            ObjectDumper.Write("By year");
            var yearGroup = allOrders.GroupBy(n => n.OrderDate.Year);
            foreach (var group in yearGroup.OrderBy(n => n.Key))
            {
                ObjectDumper.Write("-----------------------------------");
                ObjectDumper.Write(group.Key);
                ObjectDumper.Write($"Number of orders: {group.Count()}");
            }
            ObjectDumper.Write("By month & year");
            var monthYearGroup = allOrders.GroupBy(n => new { n.OrderDate.Year, n.OrderDate.Month});
            foreach (var group in monthYearGroup.OrderBy(n => n.Key.Year).ThenBy(n => n.Key.Month))
            {
                ObjectDumper.Write("-----------------------------------");
                ObjectDumper.Write(group.Key);
                ObjectDumper.Write($"Number of orders: {group.Count()}");
            }
        }

        #endregion
    }
}
