using System;
using Android.App;
using Android.Views;
using Android.Widget;

public class ShowListViewAdatper : BaseAdapter<string>
{
    private string[] _items;
    private Activity _context;

    public ShowListViewAdatper(Activity context, string[] items)
    {
        _items = items;
        _context = context;
    }

    public override long GetItemId(int position)
    {
        return position;
    }

    public override string this[int position]
    {
        get { return _items[position];}
    }

    public override int Count
    {
        get
        {
            return _items.Length;
        }
    }

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        View view = convertView; // re-use an existing view, if one is available
        if (view == null) // otherwise create a new one
        {
            view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
        }

        view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = _items[position];
        return view;
    }
}