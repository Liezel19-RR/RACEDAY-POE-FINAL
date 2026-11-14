# RaceDay API endpoint plan

All routes are prefixed with `/api`. Organiser and participant operations use the API session cookie issued by login.

## Authentication and users

| Method | Route | Access | Purpose |
|---|---|---|---|
| POST | `/auth/register` | Public | Register with a name, valid email, role, and password (minimum 8 characters). |
| POST | `/auth/login` | Public | Start a session; returns profile information including picture URL. |
| POST | `/auth/logout` | Signed in | End the session. |
| GET | `/users/me` | Signed in | Return current profile. |
| PUT | `/users/me` | Signed in | Update name, email, and phone; duplicate email returns 409. |
| POST | `/users/me/profile-picture` | Signed in | Upload a validated image as multipart field `file`. |

## Events and categories

| Method | Route | Access | Purpose |
|---|---|---|---|
| GET | `/events` | Public | List events; optional `search`, `type`, `location`, `from`, `to`, `upcomingOnly` filters. |
| GET | `/events/{id}` | Public | Event details, categories, and enrolled counts. |
| GET | `/events/mine` | Organiser | Current organiser's dashboard events. |
| POST | `/events` | Organiser | Create an event. |
| PUT | `/events/{id}` | Owner organiser | Update an event. |
| DELETE | `/events/{id}` | Owner organiser | Delete only if no enrolments exist; otherwise 409. |
| POST | `/events/{id}/banner` | Owner organiser | Upload a validated banner image as multipart field `file`. |
| GET | `/events/{eventId}/categories` | Public | List categories and active enrolment counts. |
| POST | `/events/{eventId}/categories` | Owner organiser | Create a category with distance, price, and optional capacity. |
| PUT | `/events/{eventId}/categories/{categoryId}` | Owner organiser | Update a category. |
| DELETE | `/events/{eventId}/categories/{categoryId}` | Owner organiser | Delete only if no enrolments exist; otherwise 409. |

## Enrolments and results

| Method | Route | Access | Purpose |
|---|---|---|---|
| POST | `/enrolments` | Participant | Enrol in a future event and available category. |
| GET | `/enrolments/mine` | Participant | List current participant enrolments with event/category/result data. |
| GET | `/enrolments/event/{eventId}` | Owner organiser | List event enrolments and result data. |
| PUT | `/enrolments/{id}/status` | Entry participant / owner organiser | Cancel or update an entry; entries with results cannot be cancelled. |
| POST | `/results` | Owner organiser | Record a result with status `Finished`, `DNF`, or `DQ`, positive time, and position. |
| GET | `/results/me` | Participant | List own results with event/date/category and position of total finishers. |
| GET | `/results/event/{eventId}` | Public | Event leaderboard, ordered by category, finishing position, and time. |

Invalid input returns 400, unauthenticated requests return 401, forbidden role/ownership returns 403, missing resources return 404, and conflicting state returns 409.
