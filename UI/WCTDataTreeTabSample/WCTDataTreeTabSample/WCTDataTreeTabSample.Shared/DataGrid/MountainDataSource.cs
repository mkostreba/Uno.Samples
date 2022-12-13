﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Data;
using WCTDataTreeTabSample;
using WCTDataTreeTabSample.Helpers;

namespace Microsoft.Toolkit.Uwp.SampleApp.Data
{
    [Bindable]
    public class MountainDataSource
    {
        private static ObservableCollection<MountainDataItem> _items;
        private static List<string> _mountains;
        private static CollectionViewSource groupedItems;
        private string _cachedSortedColumn = string.Empty;

        // Loading data
        public async Task<IEnumerable<MountainDataItem>> GetDataAsync()
        {
            using (var stream = await StreamHelperEx.GetEmbeddedFileStreamAsync(GetType(), "MountainSampleData.csv"))
            {
                var list = new List<MountainDataItem>();

                using (var sr = new StreamReader(stream))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split(',');

                        list.Add(
                            new MountainDataItem()
                            {
                                Rank = uint.Parse(values[0]),
                                Mountain = values[1],
                                Height_m = uint.Parse(values[2]),
                                Range = values[3],
                                Coordinates = values[4],
                                Prominence = uint.Parse(values[5]),
                                Parent_mountain = values[6],
                                First_ascent = uint.Parse(values[7]),
                                Ascents = values[8]
                            });
                    }
                }

                _items = new ObservableCollection<MountainDataItem>(
                    list
#if __WASM__        // Uncomment this line to load the sample faster in WASM interpreted mode
                    .Take(10)
#endif
                );
                return _items;
            }
        }

        // Load mountains into separate collection for use in combobox column
        public async Task<IEnumerable<string>> GetMountains()
        {
            if (_items == null || !_items.Any())
            {
                await GetDataAsync();
            }

            _mountains = _items?.OrderBy(x => x.Mountain).Select(x => x.Mountain).Distinct().ToList();

            return _mountains;
        }

        // Sorting implementation using LINQ
        public string CachedSortedColumn
        {
            get
            {
                return _cachedSortedColumn;
            }

            set
            {
                _cachedSortedColumn = value;
            }
        }

        public ObservableCollection<MountainDataItem> SortData(string sortBy, bool ascending)
        {
            _cachedSortedColumn = sortBy;
            switch (sortBy)
            {
                case "Rank":
                    if (ascending)
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Rank ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Rank descending
                                                                          select item);
                    }

                case "Parent_mountain":
                    if (ascending)
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Parent_mountain ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Parent_mountain descending
                                                                          select item);
                    }

                case "Mountain":
                    if (ascending)
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Mountain ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Mountain descending
                                                                          select item);
                    }

                case "Height_m":
                    if (ascending)
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Height_m ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Height_m descending
                                                                          select item);
                    }

                case "Range":
                    if (ascending)
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Range ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<MountainDataItem>(from item in _items
                                                                          orderby item.Range descending
                                                                          select item);
                    }
            }

            return _items;
        }

        // Grouping implementation using LINQ
        public CollectionViewSource GroupData()
        {
            ObservableCollection<GroupInfoCollection<MountainDataItem>> groups = new ObservableCollection<GroupInfoCollection<MountainDataItem>>();
            var query = from item in _items
                        orderby item
                        group item by item.Range into g
                        select new { GroupName = g.Key, Items = g };
            foreach (var g in query)
            {
                GroupInfoCollection<MountainDataItem> info = new GroupInfoCollection<MountainDataItem>();
                info.Key = g.GroupName;
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                groups.Add(info);
            }

            groupedItems = new CollectionViewSource();
            groupedItems.IsSourceGrouped = true;
            groupedItems.Source = groups;

            return groupedItems;
        }

        public class GroupInfoCollection<T> : ObservableCollection<T>
        {
            public object Key { get; set; }

            public new IEnumerator<T> GetEnumerator()
            {
                return (IEnumerator<T>)base.GetEnumerator();
            }
        }

        // Filtering implementation using LINQ
        public enum FilterOptions
        {
            All = -1,
            Rank_Low = 0,
            Rank_High = 1,
            Height_Low = 2,
            Height_High = 3
        }

        public ObservableCollection<MountainDataItem> FilterData(FilterOptions filterBy)
        {
            switch (filterBy)
            {
                case FilterOptions.All:
                    return new ObservableCollection<MountainDataItem>(_items);

                case FilterOptions.Rank_Low:
                    return new ObservableCollection<MountainDataItem>(from item in _items
                                                                      where item.Rank < 50
                                                                      select item);

                case FilterOptions.Rank_High:
                    return new ObservableCollection<MountainDataItem>(from item in _items
                                                                      where item.Rank > 50
                                                                      select item);

                case FilterOptions.Height_High:
                    return new ObservableCollection<MountainDataItem>(from item in _items
                                                                      where item.Height_m > 8000
                                                                      select item);

                case FilterOptions.Height_Low:
                    return new ObservableCollection<MountainDataItem>(from item in _items
                                                                      where item.Height_m < 8000
                                                                      select item);
            }

            return _items;
        }
    }
}
