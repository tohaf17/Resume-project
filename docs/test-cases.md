| ID     | Jira story | Перевірка                     | Передумова                                                                                                              | Вхідні дані                                                                                            | Очікуваний результат                                                                                                                                              | Тип      |
| :----- | :--------- | :---------------------------- | :---------------------------------------------------------------------------------------------------------------------- | :----------------------------------------------------------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------------- | :------- |
| TC-001 | US2E0      | Успішне створення OTP         | У БД існує заявка `TelegramAuthRequest` зі статусом `Pending` та `ExpiresAt > UtcNow`.                                  | Команда в Telegram: `/start {requestId}`, `chatId = 123456789`                                         | Прив'язано `TelegramChatId`, згенеровано 6-значний OTP, збережено SHA-256 `CodeHash`, статус змінено на `CodeIssued`, бот надіслав повідомлення з кодом у чат.    | positive |
| TC-002 | US2E0      | Прострочений request          | У БД існує заявка `TelegramAuthRequest` зі статусом `Pending`, але `ExpiresAt <= UtcNow`.                               | Команда в Telegram: `/start {requestId}`, `chatId = 123456789`                                         | Код не генерується, статус не змінюється на `CodeIssued`, бот повертає повідомлення: *"Заявка недійсна або прострочена. Спробуйте знову на сайті."*               | boundary |
| TC-003 | US2E0      | Request не знайдений          | У БД відсутній запис із переданим `requestId`.                                                                          | Команда в Telegram: `/start 00000000-0000-0000-0000-000000000000`, `chatId = 123456789`                  | Бот повертає повідомлення: *"Заявка недійсна або прострочена. Спробуйте знову на сайті."*                                                                         | negative |
| TC-004 | US4E0      | Правильний OTP                | У БД існує заявка зі статусом `CodeIssued`, `ExpiresAt > UtcNow`, `Attempts < 5`, `CodeHash` відповідає коду `"482913"`. | `POST /api/auth/telegram/verify` — `{"requestId": "{id}", "code": "482913"}`                           | Статус заявки змінюється на `Confirmed`; створюється/знаходиться `User`; зберігається хеш `RefreshToken`; повертається `200 OK` із токенами та `user`.            | positive |
| TC-005 | US4E0      | Неправильний OTP              | У БД існує заявка зі статусом `CodeIssued`, `ExpiresAt > UtcNow`, `Attempts = 0`.                                       | `POST /api/auth/telegram/verify` — `{"requestId": "{id}", "code": "111111"}`                           | Повертається `400 Bad Request` з повідомленням про невірний код; у БД `Attempts` збільшується на `1` (`Attempts = 1`), статус залишається `CodeIssued`.           | negative |
| TC-006 | US4E0      | Порожній OTP                  | У БД існує заявка зі статусом `CodeIssued`, `ExpiresAt > UtcNow`.                                                       | `POST /api/auth/telegram/verify` — `{"requestId": "{id}", "code": ""}`                                 | Повертається `400 Bad Request` (помилка валідації або невірного коду); токени не видаються.                                                                       | negative |
| TC-007 | US4E0      | OTP має не 6 цифр             | У БД існує заявка зі статусом `CodeIssued`, `ExpiresAt > UtcNow`.                                                       | `POST /api/auth/telegram/verify` — `{"requestId": "{id}", "code": "12345"}` або `"1234567"`            | Повертається `400 Bad Request`; хеш не збігається (або спрацьовує валідація довжини), лічильник `Attempts` збільшується, токени не видаються.                     | boundary |
| TC-008 | US4E0      | П’ята неправильна спроба      | У БД існує заявка зі статусом `CodeIssued`, `ExpiresAt > UtcNow`, `Attempts = 4`.                                       | `POST /api/auth/telegram/verify` — `{"requestId": "{id}", "code": "000000"}`                           | `Attempts` стає `5`; статус заявки автоматично змінюється на `Expired`; повертається `400 Bad Request` із повідомленням про перевищення спроб.                    | boundary |
| TC-009 | US4E0      | Прострочений OTP              | У БД існує заявка зі статусом `CodeIssued`, `Attempts < 5`, але `ExpiresAt <= UtcNow`.                                  | `POST /api/auth/telegram/verify` — `{"requestId": "{id}", "code": "482913"}`                           | Статус заявки оновлюється на `Expired`; повертається `400 Bad Request` із повідомленням про вичерпання часу дії коду.                                             | boundary |
| TC-010 | US5E0      | Успішна ротація Refresh Token | У таблиці `RefreshTokens` існує активний токен (`ExpiresAt > UtcNow`, `RevokedAt == null`) для існуючого `User`.        | `POST /api/auth/refresh` — `{"refreshToken": "{validRawRefreshToken}"}`                                | Старий токен у БД отримує `RevokedAt = UtcNow`; створюється новий `RefreshToken`; повертається `200 OK` з новою парою `accessToken` та `refreshToken`.            | positive |
| TC-011 | US5E0      | Порожній Refresh Token        | Користувач має активну сесію або надсилає порожній запит.                                                               | `POST /api/auth/refresh` — `{"refreshToken": ""}`                                                      | Повертається `401 Unauthorized` (або `400 Bad Request` при модельній валідації); нові токени не генеруються.                                                      | negative |
| TC-012 | US5E0      | Прострочений Refresh Token    | У таблиці `RefreshTokens` існує запис із `RevokedAt == null`, але `ExpiresAt <= UtcNow`.                                | `POST /api/auth/refresh` — `{"refreshToken": "{expiredRawRefreshToken}"}`                              | Повертається `401 Unauthorized`; нова пара токенів не видається.                                                                                                  | boundary |
| TC-013 | US5E0      | Відкликаний Refresh Token     | У таблиці `RefreshTokens` існує запис із `ExpiresAt > UtcNow`, але `RevokedAt != null` (вже використаний / logout).     | `POST /api/auth/refresh` — `{"refreshToken": "{revokedRawRefreshToken}"}`                              | Повертається `401 Unauthorized`; повторна ротація заблокована.                                                                                                    | negative |
| TC-014 | US7E0      | Успішний вхід Admin           | У таблиці `Users` існує запис з `Email = "admin@resumebuilder.app"`, валідним BCrypt-хешем пароля та `Role = Admin`.    | `POST /api/admin/auth/login` — `{"email": "admin@resumebuilder.app", "password": "Admin123!"}`         | Повертається `200 OK` із `accessToken` (містить claim `role: Admin`), `refreshToken` (хеш збережено у `RefreshTokens`) та даними профілю адміна.                  | positive |
| TC-015 | US7E0      | Невірний пароль або роль User | У БД існує адміністратор або звичайний користувач із `Role = User` та валідним паролем.                                 | `POST /api/admin/auth/login` — невірний пароль адміна АБО валідні дані користувача з `Role = User`     | Повертається `401 Unauthorized`; доступ заборонено, токени не видаються.                                                                                          | negative |






## Класи еквівалентності та граничні значення

### Для OTP

| Параметр     | Клас         | Приклад   |
| :----------- | :----------- | :-------- |
| Довжина коду | Менше 6      | `12345`   |
| Довжина коду | Рівно 6      | `123456`  |
| Довжина коду | Більше 6     | `1234567` |
| Значення     | Лише цифри   | `482913`  |
| Значення     | Є літери     | `12A456`  |

### Для кількості спроб

| Значення | Очікування                |
| :------- | :------------------------ |
| 0        | Заявка активна            |
| 4        | Залишилася остання спроба |
| 5        | Статус Expired            |
| Більше 5 | Нові спроби заборонені    |

### Для Refresh Token

| Значення                        | Очікування |
| :------------------------------ | :--------- |
| `RevokedAt == null`, дата майбутня | Валідний   |
| `ExpiresAt == UtcNow`           | Недійсний  |
| `ExpiresAt < UtcNow`            | Недійсний  |
| `RevokedAt != null`             | Недійсний  |