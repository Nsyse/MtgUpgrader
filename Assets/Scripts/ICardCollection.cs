using System;
using System.Collections.Generic;

public interface ICardCollection : ICollection<Card>
{
    int Count { get; }
    bool Contains(Card card);

    bool IsEmpty();
}