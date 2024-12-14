using GpChatBot.Database.Model;
using GpChatBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GpChatBot.Database;

public class ChatDbContext : DbContext
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<UserMessage> UserMessages { get; set; }
    public DbSet<BotMessage> BotMessages { get; set; }

    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Chat>()
            .HasKey(ch => ch.Id);

        modelBuilder.Entity<Chat>()
            .HasMany(ch => ch.UserMessages)
            .WithOne(um => um.Chat)
            .HasForeignKey(um => um.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Chat>()
            .Property(um => um.ClientIpAddress)
            .IsRequired();

        modelBuilder.Entity<UserMessage>()
            .HasKey(um => um.Id);

        modelBuilder.Entity<UserMessage>()
            .Property(um => um.Message)
            .IsRequired();

        modelBuilder.Entity<BotMessage>()
            .HasKey(um => um.Id);

        modelBuilder.Entity<BotMessage>()
            .Property(um => um.Message)
            .IsRequired();
    }
}