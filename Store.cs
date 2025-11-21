using System;
using System.Collections.Generic;
using System.Linq;

internal class Program
{
    static void Main(string[] args)
    {
        Product iPhone12 = new Product("IPhone 12");
        Product iPhone11 = new Product("IPhone 11");

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

public class Product
{
    public Product(string name)
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
    List<Product> TakeProduct(Product products, int count);
    void Delive(Product product, int count);
    int GetAvailableCount(Product product);
}

public class Cart
{
    private readonly IWarehouse _warehouse;
    private Dictionary<Product, int> _products = new Dictionary<Product, int>();

    public Cart(IWarehouse warehouse)
    {
        _warehouse = warehouse;
    }

    public void Show()
    {
        Console.WriteLine("Товары в корзине:\n");

        foreach (var item in _products)
        {
            Console.WriteLine($"{item.Key.Name} - {item.Value}");
        }
    }

    public void Add(Product product, int count)
    {
        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (_warehouse.GetAvailableCount(product) < count)
        {
            throw new InvalidOperationException();
        }

        if (_products.ContainsKey(product))
        {
            _products[product] += count;
        }
        else
        {
            _products[product] = count;
        }
    }

    public Order Order()
    {
        foreach (var item in _products)
        {
            _warehouse.TakeProduct(item.Key, item.Value);
        }

        return new Order();
    }
}

public class Warehouse : IWarehouse
{
    private List<Cell> _cells = new List<Cell>();

    public void Show()
    {
        foreach (Cell cell in _cells)
        {
            Console.WriteLine($"Товар:{cell.Product.Name} - {cell.Count}");
        }
    }

    public int GetAvailableCount(Product product)
    {
        var cell = _cells.FirstOrDefault(cell => cell.Product.Name == product.Name);

        if (cell == null)
            return 0;
        else
            return cell.Count;
    }

    public List<Product> TakeProduct(Product good, int count)
    {
        var cell = _cells.FirstOrDefault(cell => cell.Product.Name == good.Name);

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
            List<Product> products = new List<Product>();
            cell.Take(count);

            for (int i = 0; i < count; i++)
            {
                products.Add(new Product(cell.Product.Name));
            }

            return products;
        }
    }

    public void Delive(Product product, int count)
    {
        var cell = _cells.FirstOrDefault(cell => cell.Product.Name == product.Name);

        if (cell == null)
        {
            _cells.Add(new Cell(product, count));
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
}

public class Cell
{
    public Cell(Product product, int count)
    {
        Product = product;
        Count = count;
    }

    public Product Product { get; private set; }
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

public class Order
{
    private static readonly Random _random = new Random();

    public Order()
    {
        Paylink = GeneratePaylink();
    }

    public string Paylink { get; private set; }

    private string GeneratePaylink()
    {
        // Генерация случайной ссылки для оплаты
        return $"https://payment.com/{_random.Next(1000, 9999)}";
    }
}