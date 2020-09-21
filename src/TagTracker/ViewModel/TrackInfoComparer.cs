using System;
using System.Collections;

namespace TagTracker.ViewModel
{
    public class TrackInfoComparer : IComparer
    {
        private readonly Func<TrackInfoViewModel, object> _selector;
        private readonly int _sortOrder;

        /// <summary>
        /// Initialises a new instance of the <see cref="TrackInfoComparer"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="isAscending">If set to <c>true</c> [is ascending].</param>
        public TrackInfoComparer(Func<TrackInfoViewModel, object> selector, bool isAscending)
        {
            _selector = selector;
            _sortOrder = isAscending ? 1 : -1;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/>
        /// and <paramref name="y"/>, as shown in the following table.Value 
        /// Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// Neither <paramref name="x"/> nor <paramref name="y"/> implements the
        /// <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and
        /// <paramref name="y"/> are of different types and neither one can handle comparisons with the other.
        /// </exception>
        public int Compare(object x, object y)
        {
            TrackInfoViewModel track1 = x as TrackInfoViewModel;
            TrackInfoViewModel track2 = y as TrackInfoViewModel;

            if (track1 != null && track2 != null)
            {
                int result = ((IComparable)_selector(track1)).CompareTo(_selector(track2)) * _sortOrder;
                if (result == 0)
                {
                    return string.Compare(
                        track1.TrackInfo.FileName,
                        track2.TrackInfo.FileName,
                        StringComparison.CurrentCultureIgnoreCase);
                }
                return result;
            }

            return 0;
        }
    }
}
