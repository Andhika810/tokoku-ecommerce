using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public static class DbSeeder
{
    private static string Img(string seed) => $"https://picsum.photos/seed/{seed}/600/450";

    public static async Task SeedAsync(AppDbContext db)
    {
        async Task<Category> Cat(string name)
        {
            var c = await db.Categories.FirstOrDefaultAsync(x => x.Name == name);
            if (c is null)
            {
                c = new Category { Name = name };
                db.Categories.Add(c);
                await db.SaveChangesAsync();
            }
            return c;
        }

        var elektronik = await Cat("Elektronik");
        var fashion = await Cat("Fashion");
        var rumah = await Cat("Rumah Tangga");
        var olahraga = await Cat("Olahraga");

        var items = new (string Name, string Desc, decimal Price, int Stock, Category Cat)[]
        {
            ("Headphone Bluetooth", "Noise cancelling, baterai 30 jam", 450000, 25, elektronik),
            ("Keyboard Mekanikal", "Switch biru, RGB, layout 87 tombol", 520000, 15, elektronik),
            ("Smartwatch Sport", "Pelacak detak jantung dan tidur", 750000, 12, elektronik),
            ("Power Bank 20.000 mAh", "Fast charging dua port USB", 280000, 40, elektronik),
            ("Kaos Polos Premium", "Katun combed 30s, nyaman dipakai", 89000, 60, fashion),
            ("Jaket Hoodie", "Bahan fleece tebal dan hangat", 245000, 30, fashion),
            ("Sepatu Sneakers", "Ringan, cocok untuk harian", 399000, 18, fashion),
            ("Tas Ransel Laptop", "Muat laptop 15 inci, anti air", 215000, 22, fashion),
            ("Lampu Meja LED", "3 mode cahaya, hemat energi", 125000, 35, rumah),
            ("Botol Minum Stainless", "Tahan panas dan dingin 12 jam", 95000, 50, rumah),
            ("Matras Yoga", "Anti slip, tebal 8 mm", 150000, 20, olahraga),
            ("Dumbbell Set 10 kg", "Pasangan, lapis karet", 330000, 8, olahraga),
        };

        var existing = await db.Products.Select(p => p.Name).ToListAsync();
        foreach (var i in items.Where(i => !existing.Contains(i.Name)))
        {
            db.Products.Add(new Product
            {
                Name = i.Name, Description = i.Desc, Price = i.Price,
                Stock = i.Stock, CategoryId = i.Cat.Id, ImageUrl = Img(i.Name)
            });
        }

        // lengkapi gambar untuk produk lama (misalnya Mouse)
        foreach (var p in await db.Products.Where(p => p.ImageUrl == null).ToListAsync())
            p.ImageUrl = Img(p.Name);

        await db.SaveChangesAsync();
    }
}