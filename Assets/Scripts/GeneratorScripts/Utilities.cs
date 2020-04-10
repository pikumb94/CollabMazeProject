using System.Collections;
using System.Collections.Generic;

public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Pair<T, U>)obj);
    }

    public bool Equals(Pair<T, U> obj)
    {
        return obj != null && obj == this;
    }

    public override int GetHashCode()
    {
        return First.GetHashCode() ^ Second.GetHashCode();
    }


    public static bool operator ==(Pair<T, U> p1, Pair<T, U> p2)
    {
        return ((dynamic)p1.First == p2.First && (dynamic)p1.Second == p2.Second);
    }

    public static bool operator !=(Pair<T, U> p1, Pair<T, U> p2)
    {
        return !(p1== p2);
    }

    public static bool operator <(Pair<T, U> p1, Pair<T, U> p2)
    {
        return ((dynamic)p1.First < p2.First && (dynamic)p1.Second < p2.Second);
    }

    public static bool operator >(Pair<T, U> p1, Pair<T, U> p2)
    {
        return !(p1<p2) && p1!=p2;
    }
};
