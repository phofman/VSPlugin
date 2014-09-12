﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlackBerry.Package.ToolWindows.ViewModel
{
    /// <summary>
    /// Base class for all view model items.
    /// </summary>
    public abstract class BaseViewItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isExpanded;
        private bool _isLoading;
        private ImageSource _imageSource;
        private object _content;

        protected static readonly BaseViewItem ExpandPlaceholder = new ProgressViewItem(null, "X-X-X");

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseViewItem(TargetNavigatorViewModel viewModel)
        {
            ViewModel = viewModel;
            Children = new ObservableCollection<BaseViewItem>();
        }

        #region Properties

        public TargetNavigatorViewModel ViewModel
        {
            get;
            private set;
        }

        public abstract string Name
        {
            get;
        }

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    NotifyPropertyChanged("ImageSource");
                }
            }
        }

        public ObservableCollection<BaseViewItem> Children
        {
            get;
            private set;
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;

                    if (value)
                    {
                        Refresh();
                    }
                    else
                    {
                        Collapse();
                    }

                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                    if (value)
                    {
                        ViewModel.SelectedItem = this;
                        Selected();
                    }
                }
            }
        }

        public object Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        #endregion

        protected void AddExpandablePlaceholder()
        {
            Children.Add(ExpandPlaceholder);
        }

        public void Refresh()
        {
            if (!_isLoading)
            {
                _isLoading = true;

                Children.Clear();
                var progressItem = CreateProgressPlaceholder();
                if (progressItem != null)
                {
                    Children.Add(progressItem);
                }

                // retrieve items to fill-in the list asynchronously:
                ThreadPool.QueueUserWorkItem(InternalLoadItems);
            }
        }

        public void Collapse()
        {
            IsExpanded = false;
        }

        private void InternalLoadItems(object state)
        {
            try
            {
                LoadItems();
            }
            catch (Exception ex)
            {
                // to make sure, all async loads always 'complete', when exception crashed them...
                OnItemsLoaded(new BaseViewItem[] { new MessageViewItem(ViewModel, ex) });
            }
        }

        /// <summary>
        /// Method called after asynchronous items were loaded to populate them to the UI.
        /// </summary>
        protected void OnItemsLoaded(BaseViewItem[] items)
        {
            OnItemsLoaded(items, null);
        }

        /// <summary>
        /// Method called after asynchronous items were loaded to populate them to the UI.
        /// </summary>
        protected void OnItemsLoaded(BaseViewItem[] items, object state)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                InternalRefreshItemsLoaded(items, state);
            }
            else
            {
                dispatcher.BeginInvoke(DispatcherPriority.Background, new Action<BaseViewItem[], object>(InternalRefreshItemsLoaded), items, state);
            }
        }

        /// <summary>
        /// Refreshes the collection of child-items. Since it automatically fires the collection-changed events, it should be only called from the UI thread.
        /// </summary>
        private void InternalRefreshItemsLoaded(BaseViewItem[] items, object state)
        {
            Children.Clear();

            // did we loaded items correctly?
            if (items == null)
            {
                var error = CreateErrorPlaceholder();
                if (error != null)
                {
                    Children.Add(error);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    Children.Add(item);
                }
            }

            ItemsCompleted(state);
            _isLoading = false;
        }

        protected virtual BaseViewItem CreateProgressPlaceholder()
        {
            return new ProgressViewItem(ViewModel, "Loading...");
        }

        protected virtual BaseViewItem CreateErrorPlaceholder()
        {
            return new ProgressViewItem(ViewModel, "Failed to list items");
        }

        /// <summary>
        /// Method invoked on background thread to list children of this item.
        /// It can take as much time as needed. The progress indicator is returned by CreateProgressPlaceholder() call.
        /// </summary>
        protected virtual void LoadItems()
        {
            // by default display empty list:
            OnItemsLoaded(new BaseViewItem[0]);
        }

        /// <summary>
        /// Invoked on UI thread, when all items have been populated.
        /// </summary>
        protected virtual void ItemsCompleted(object state)
        {
        }

        /// <summary>
        /// Invoked on UI thread, when current ViewItem has been selected.
        /// </summary>
        protected virtual void Selected()
        {
        }

        /// <summary>
        /// Updates the Content property from any thread.
        /// </summary>
        protected void UpdateContent(object content)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                InternalUpdateContent(content);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action<object>(InternalUpdateContent), content);
            }
        }

        private void InternalUpdateContent(object content)
        {
            Content = content;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies that value of specified property has changed.
        /// </summary>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}