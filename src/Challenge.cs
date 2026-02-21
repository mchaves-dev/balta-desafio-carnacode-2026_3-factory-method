// DESAFIO: Sistema de Notificações Multi-Canal
// PROBLEMA: Uma aplicação de e-commerce precisa enviar notificações por diferentes canais
// (Email, SMS, Push, WhatsApp) dependendo da preferência do cliente e tipo de notificação
// O código atual viola o Open/Closed Principle ao usar condicionais para criar notificações

using System;

namespace DesignPatternChallenge
{

    public abstract class Notification
    {
        public abstract void Send(NotificationMessage notificationMessage);
    }

    public class NotificationMessage
    {
        public string Recipient { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public interface INotificationFactory
    {
        Notification Create(string provider);
    }

    public sealed class NotificationFactory : INotificationFactory
    {
        private readonly Dictionary<string, Notification> _notifications;

        public NotificationFactory()
        {
            _notifications = new Dictionary<string, Notification>(StringComparer.OrdinalIgnoreCase)
            {
                { "email", new EmailNotification() },
                { "sms", new SmsNotification() },
                { "push", new PushNotification() },
                { "whatsapp", new WhatsAppNotification() },
                {"telegram",new TelegramNotification()}
            };
        }

        public Notification Create(string provider)
        {
            if (_notifications.TryGetValue(provider, out var notification))
            {
                return notification;
            }

            throw new NotSupportedException($"Provider '{provider}' não suportado.");
        }
    }

    public sealed class NotificationManager
    {
        private readonly INotificationFactory _notificationFactory;

        public NotificationManager(INotificationFactory notificationFactory)
        {
            _notificationFactory = notificationFactory;
        }

        public void SendOrderConfirmation(string recipient, string orderNumber, string notificationType)
        {
            var notification = _notificationFactory.Create(notificationType);

            var message = new NotificationMessage()
            {
                Recipient = recipient,
                Title = "Pedido Confirmado",
                Content = $"Seu pedido {orderNumber} foi confirmado!"
            };

            notification.Send(message);
        }

        public void SendShippingUpdate(string recipient, string trackingCode, string notificationType)
        {
            var notification = _notificationFactory.Create(notificationType);

            var message = new NotificationMessage()
            {
                Recipient = recipient,
                Title = "Pedido Enviado",
                Content = $"Seu pedido foi enviado! Código de rastreamento: {trackingCode}"
            };

            notification.Send(message);
        }

        public void SendPaymentReminder(string recipient, decimal amount, string notificationType)
        {
            var notification = _notificationFactory.Create(notificationType);

            var message = new NotificationMessage()
            {
                Recipient = recipient,
                Title = "Lembrete de Pagamento",
                Content = $"Você tem um pagamento pendente de R$ {amount:N2}"
            };

            notification.Send(message);
        }
    }

    // Classes concretas de notificação
    public sealed class EmailNotification : Notification
    {
        public override void Send(NotificationMessage notificationMessage)
        {
            Console.WriteLine($"📧 Enviando Email para {notificationMessage.Recipient}");
            Console.WriteLine($"   Assunto: {notificationMessage.Title}");
            Console.WriteLine($"   Mensagem: {notificationMessage.Content}");
        }
    }

    public sealed class SmsNotification : Notification
    {
        public override void Send(NotificationMessage notificationMessage)
        {
            Console.WriteLine($"📱 Enviando SMS para {notificationMessage.Recipient}");
            Console.WriteLine($"   Mensagem: {notificationMessage.Content}");
        }
    }

    public sealed class PushNotification : Notification
    {
        public override void Send(NotificationMessage notificationMessage)
        {
            Console.WriteLine($"🔔 Enviando Push para dispositivo {notificationMessage.Recipient}");
            Console.WriteLine($"   Título: {notificationMessage.Title}");
            Console.WriteLine($"   Mensagem: {notificationMessage.Content}");
        }
    }

    public sealed class WhatsAppNotification : Notification
    {
        public override void Send(NotificationMessage notificationMessage)
        {
            Console.WriteLine($"💬 Enviando WhatsApp para {notificationMessage.Recipient}");
            Console.WriteLine($"   Mensagem: {notificationMessage.Content}");
            Console.WriteLine($"   Template: {notificationMessage.Title}");
        }
    }

    public class TelegramNotification : Notification
    {
        public override void Send(NotificationMessage notificationMessage)
        {
            Console.WriteLine($"💬 Enviando Telegram para {notificationMessage.Recipient}");
            Console.WriteLine($"   Mensagem: {notificationMessage.Content}");
            Console.WriteLine($"   Template: {notificationMessage.Title}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Notificações ===\n");

            var manager = new NotificationManager(new NotificationFactory());

            // Cliente 1 prefere Email
            manager.SendOrderConfirmation("cliente@email.com", "12345", "email");
            Console.WriteLine();

            // Cliente 2 prefere SMS
            manager.SendOrderConfirmation("+5511999999999", "12346", "sms");
            Console.WriteLine();

            // Cliente 3 prefere Push
            manager.SendShippingUpdate("device-token-abc123", "BR123456789", "push");
            Console.WriteLine();

            // Cliente 4 prefere WhatsApp
            manager.SendPaymentReminder("+5511888888888", 150.00m, "whatsapp");

            Console.WriteLine();
            // Cliente 4 prefere WhatsApp
            manager.SendPaymentReminder("+5511888888888", 150.00m, "telegram");
        }
    }
}