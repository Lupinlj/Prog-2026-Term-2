# API Endpoint Plan

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/auth/register | Register a new user account | None (public) | {name, email, password, role} | 201 Created - new registration: 400 Bad Request - if required fields missing |
| POST | /api/auth/login | Logs in an existing user and starts their session | None (public) | {email, password} | 200 OK - session/token with user role; 401 Unauthorised - invalid credentials |
| GET | /api/profile | Returns the logged-in user's own profile details | Any (logged in) | None | 200 OK - Returns user profile details (name, email); 401 Unauthorized - no valid session/not logged in |
| PUT | /api/profile | Updates the logged-in user's own profile details | Any (logged in) | {name, email} | 200 OK - updated profile details; 400 Bad Request - invalid or missing fields |
| POST | /api/events | Create a new events | Organiser | {name, description, date, location, distance, eventType} | 201 Created - event created; 403 Forbidden - only Organisers are allowed |
| GET | /api/events | List all events | Any (logged in) | None | 200 OK - array of events; 401 Unauthorized - no valid session|
| PUT | /api/events/{id} | Update an existing event | Organiser | {name, description, date, location, distance, eventType} | 200 OK - updated event; 404 Not Found - event doesn't exist; 403 Forbidden - only Organisers are allowed |
| DELETE | /api/events/{id} | Removes an event | Organiser | None | 200 OK - confirmation; 404 Not Found - event doesn't exist; 403 Forbidden - only Organisers can delete event |
| POST | /api/events/{eventId}/categories | New event category gets added by Organiser | Organiser | {categoryName} | 201 Created - new category; 403 Forbidden - only Organisers allowed |
| GET | /api/events/{eventId}/categories | List all categories for a specific event | Any (logged in) | None | 200 OK - array of categories; 401 Unauthorized - no valid session |
| POST | /api/events/{eventId}/enrolments | Select a category to enrol in the event | Participant | {categoryId} | 201 Created - enrolment record; 400 Bad Request - invalid category |
| GET | /api/enrolments/mine | Returns a participant's enrolment history | Participant | None | 200 OK - array of enrolments; 401 Unauthorized - no valid session |
| GET | /api/events/{eventId}/enrolments | Organiser views all enrolments for one of their events | Organiser | None | 200 OK - array of enrolments; 403 Forbidden - only Organisers allowed | 
| POST | /api/enrolments/{enrolmentId}/result | Organiser captures results for finish time and position for a participant Organiser | {finishTime, position} | 201 Created - result recorded; 403 Forbidden - only Organisers are allowed |
| GET | /api/results/mine | Shows participant's logged-in results | Participant | None | 200 OK - array of results; 401 Unauthorized - no valid session
