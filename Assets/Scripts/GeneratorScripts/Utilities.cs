using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility classes and helper functions.
/// </summary>

/*
[System.Serializable]

public class Pair<T, U>
{
    [SerializeField]
    public T x;
    [SerializeField]
    public U y;

    public Pair()
    {
    }

    public Pair(T x, U y)
    {
        this.x = x;
        this.y = y;
    }

    

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
        return x.GetHashCode() ^ y.GetHashCode();
    }


    public static bool operator ==(Pair<T, U> p1, Pair<T, U> p2)
    {
        return ((dynamic)p1.x == p2.x && (dynamic)p1.y == p2.y);
    }

    public static bool operator !=(Pair<T, U> p1, Pair<T, U> p2)
    {
        return !(p1== p2);
    }

    public static bool operator <(Pair<T, U> p1, Pair<T, U> p2)
    {
        return ((dynamic)p1.x < p2.x && (dynamic)p1.y < p2.y);
    }

    public static bool operator >(Pair<T, U> p1, Pair<T, U> p2)
    {
        return !(p1<p2) && p1!=p2;
    }
};*/
