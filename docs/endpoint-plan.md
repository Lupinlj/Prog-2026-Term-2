# API Endpoint Plan

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/auth/register | Register a new user account | None (public) | {name, email, password, role} | 201 Created - new registration: 400 Bad Request - if required fields missing |
| POST | /api/auth/login | Logs in an existing user and starts their session | None (public) | {email, password} | 200 OK - session/token with user role; 401 Unauthorised - invalid credentials |
| GET | /api/profile | Returns the logged-in user's own profile details | Any (logged in) | None | 200 OK - Returns user profile details (name, email); 401 Unauthorized - no valid session/not logged in |
| PUT | /api/profile | Updates the logged-in user's own profile details | Any (logged in) | {name, email} | 200 OK - updated profile details; 400 Bad Request - invalid or missing fields |
