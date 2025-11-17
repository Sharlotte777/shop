using System;
using System.Collections.Generic;
using System.Linq;

internal class Program
{
    static void Main(string[] args)
    {
        Good iPhone12 = new Good("IPhone 12");
        Good iPhone11 = new Good("IPhone 11");

        Warehouse warehouse = new Warehouse();

        Shop shop = new Shop(warehouse);

        warehouse.Delive(iPhone12, 10);
        warehouse.Delive(iPhone11, 1);

        warehouse.Show();

        Cart cart = shop.Cart();
        cart.Add(iPhone12, 4);
        cart.Add(iPhone11, 3); 

        cart.Show();

        Console.WriteLine(cart.Order().Paylink);

        cart.Add(iPhone12, 9);
    }
}

public class Good
{
    public string Name { get; private set; }

    public Good(string name)
    {
        if (name == null || name == "")
        {
            throw new ArgumentNullException();
        }
        else
        {
            Name = name;
        }
    }
}

public class Cart
{
    public readonly Shop Shop;

    private List<Good> _goods = new List<Good>();

    public Cart(Shop shop)
    {
        Shop = shop;
    }

    public void Show()
    {
        Console.WriteLine("Goods:\n");

        foreach (Good good in _goods)
        {
            Console.WriteLine($"{good.Name}");
        }
    }

    public void Add(Good good, int count)
    {
        List<Good> boughtGoods = Shop.Buy(good, count);

        if (boughtGoods != null)
        {
            foreach (Good boughtGood in boughtGoods)
            {
                _goods.Add(boughtGood);
            }
        }
    }

    public Shop Order() => Shop;
}

public class Warehouse
{
    private List<Cell> _cells = new List<Cell>();

    public void Show()
    {
        foreach (Cell cell in _cells)
        {
            Console.WriteLine($"Good:{cell.Good.Name} - {cell.Count}");
        }
    }

    public List<Good> TakeGood(Good good, int count)
    {
        var cell = _cells.FirstOrDefault(cell => cell.Good.Name == good.Name);

        if (cell == null)
        {
            throw new NullReferenceException();
        }

        if (cell.Count < count)
        {
            throw new ArgumentOutOfRangeException();
        }
        else
        {
            List<Good> goods = new List<Good>();
            cell.Take(count);

            for (int i = 0; i < count; i++)
            {
                goods.Add(new Good(cell.Good.Name));
            }

            return goods;
        }
    }

    public void Delive(Good good, int count)
    {
        var cell = _cells.FirstOrDefault(cell => cell.Good.Name == good.Name);

        if (cell == null)
        {
            _cells.Add(new Cell(good, count));
            return;
        }

        cell.AddCount(count);
    }
}

public class Shop
{
    public string Paylink { get; private set; } = "Random";

    private Warehouse _warehouse;

    public Shop(Warehouse warehouse)
    {
        _warehouse = warehouse;
    }

    public Cart Cart() => new Cart(this);

    public List<Good> Buy(Good good, int count) => _warehouse.TakeGood(good, count);
}

public class Cell
{
    public readonly Good Good;

    public int Count { get; private set; }

    public Cell(Good good, int count)
    {
        Good = good;
        Count = count;
    }

    public void AddCount(int count)
    {
        if (count > 0)
        {
            Count += count;
        }
    }

    public void Take(int count)
    {
        if (count > 0 && count <= Count)
        {
            Count -= count;
        }
    }
}