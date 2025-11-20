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

        Cart cart = new Cart(warehouse);
        cart.Add(iPhone12, 4);
        cart.Add(iPhone11, 3); 

        cart.Show();

        Console.WriteLine(cart.Order().Paylink);

        cart.Add(iPhone12, 9);
    }
}

public class Good
{
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

    public string Name { get; private set; }
}

public interface IWarehouse
{
    List<Good> TakeGood(Good good, int count);
    void Delive(Good good, int count);
}

public class Cart
{
    private readonly IWarehouse _warehouse;
    private Dictionary<Good, int> _goods = new Dictionary<Good, int>();

    public Cart(IWarehouse warehouse)
    {
        _warehouse = warehouse;
    }

    public void Show()
    {
        Console.WriteLine("Товары в корзине:\n");

        foreach (var item in _goods)
        {
            Console.WriteLine($"{item.Key.Name} - {item.Value}");
        }
    }

    public void Add(Good good, int count)
    {
        if (_goods.ContainsKey(good))
        {
            _goods[good] += count;
        }
        else
        {
            _goods[good] = count;
        }
    }

    public Shop Order()
    {
        foreach (var item in _goods)
        {
            _warehouse.TakeGood(item.Key, item.Value);
        }

        return new Shop(_warehouse);
    }
}

public class Warehouse : IWarehouse
{
    private List<Cell> _cells = new List<Cell>();

    public void Show()
    {
        foreach (Cell cell in _cells)
        {
            Console.WriteLine($"Товар:{cell.Good.Name} - {cell.Count}");
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
    private readonly IWarehouse _warehouse;

    public Shop(IWarehouse warehouse)
    {
        _warehouse = warehouse;
    }

    public string Paylink { get; private set; } = "Random";
}

public class Cell
{
    public Cell(Good good, int count)
    {
        Good = good;
        Count = count;
    }

    public Good Good { get; private set; }
    public int Count { get; private set; }

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