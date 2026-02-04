# RabbitMQ

В этом репозитории находятся примеры использования RabbitMQ реализованные на C#. Запустить примеры или посмотреть их описание можно на сайте [rabbitmq.gmike.com](https://gmike.ru/rabbitmq.html).

Проект собирается и развертывается в Docker. Один контейнер это RabbitMQ, а второй это Web API для запуска примеров.

Структура проекта:

| Папка             | Описание                                    |
|-------------------|---------------------------------------------|
| .build            | Скрипты для развертывания примеров в Docker |
| RabbitMQ.Examples | Примеры использования RabbitQM              |
| RabbitMQ.WebAPI   | Web API для запуска примеров                |

### Запуск локально

После запуска локально, RabbitMQ UI будет доступен по адресу <http://localhost:15185>. Логин и пароль установлены по умолчанию guest/guest.

```text
// Собрать контейнеры локально 
docker-compose build

// Запустить собранные контейнеры
docker-compose up -d 

// Вызвать примеры через HTTP запросы. Например:
// POST http://localhost:5185/api/produce
// {
//   "type": "simple", // simple, worker, publish, routing
//   "message": "Hallo world!"
// }
```

### Развертывание в Beget

```text
// Собрать контейнер Web API локально
docker-compose build rabbit_api  

// Сохранить собранный контейнер в файле
docker save -o example_rabbit_api.tar example_rabbit_api

// Скопировать файл с контейнером на сервер
// Загрузить контейнер в Docker
docker load < example_rabbit_api.tar

// Скопировать файл docker-compose.yaml на сервер  
// Запустить контейнеры
docker-compose up -d
```
