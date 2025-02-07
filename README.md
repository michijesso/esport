1) Перед запуском неодходимо развернуть локально контейнеры Kafka и PostgreSQL. Все порты в appsettings.json стандартные.
2) Запустить сервис Esport.Kafka.Subscriber
3) Запустить сервис Esport.Web
4) Запустить сервис Esport.Generator
5) Для подключения по WebSocket использовать роуты:
- ws://localhost:5088/api/ws/getAllEvents - подписка на все события;
- ws://localhost:5088/api/ws/getEventById/{eventId} - подписка на конкретное событие (id можно посмотреть в базе после того как несколько событий уже сгенерировалось)
