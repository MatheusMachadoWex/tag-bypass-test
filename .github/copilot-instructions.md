# GitHub Copilot Code Review Instructions

**Purpose:**  
These instructions guide GitHub Copilot during pull request reviews.  
Copilot should **add comments** (never block PRs) when it detects violations or potential improvements based on the rules below.

---

## 🔧 Review Behavior

- Always prefer **adding a comment** instead of skipping uncertain cases.  
- If unsure, prefix your comment with:  
  > Possible issue: …

- When commenting, use the format below:

### 💬 Comment Template
> **[Rule Code: M-010]** Description of the issue and what should be fixed or verified.

### 💬 Example Comments
- **Critical (MUST)**  
  > **[M-002]** The OpenAPI specification seems missing or outdated. Please ensure it exists and reflects the latest implementation.
- **Recommended (SHOULD)**  
  > **[S-009]** This field returns `null`. Consider omitting it or using an empty array (`[]`).

---

## 🎯 Overall Goal

Focus your review on:
- API consistency and contract correctness  
- REST and JSON design conventions  
- Error handling and compatibility  
- Security and reliability of API definitions  

Avoid style or formatting comments unless they affect readability or correctness.

---

## 🚨 MUST Requirements (Critical Issues — Always Comment)

### 🧩 Naming Conventions

When reviewing API specs or code:
- **M-010**: If JSON properties are **not in `snake_case`** (`^[a-z_][a-z_0-9]*$`), comment to fix them.  
- **M-011**: If URL paths are **not in kebab-case**, comment (e.g., `/shipment-orders/{order-id}`).  
- **M-012**: If query parameters are **not in `snake_case`**, add a comment.  
- **M-013**: If resource names are **singular**, comment to make them plural (e.g., `/customers` not `/customer`).  
- **M-014**: Comment if URLs contain trailing slashes (e.g., `/orders/` → `/orders`).

---

### 🌐 API Design

- **M-001**: If implementation lacks an OpenAPI spec, comment to add it.  
- **M-002**: If the OpenAPI spec is outdated or incomplete, comment to update it (must be YAML).  
- **M-006**: If any endpoint uses plain HTTP or exposes credentials in URLs, flag as critical.  
- **M-007**: If backward compatibility may be broken, comment clearly.  
- **M-008**: If the top-level response is not a JSON object (array/primitive), comment to fix.  
- **M-009**: Comment if URI versioning is missing or incorrect (should be `/v{MAJOR}/resource`, e.g., `/v1/customers`).  
- **M-015**: If URLs are **action-oriented** instead of resource-oriented (e.g., `/createUser`), comment to redesign.

---

### 🧾 HTTP Methods & Status Codes

When reviewing endpoint definitions:
- **M-018**: Verify HTTP method usage:
  - `GET` → read-only  
  - `POST` → create/process  
  - `PUT` → full update  
  - `PATCH` → partial update  
  - `DELETE` → remove  
  Comment if mismatched.
- **M-019**: Ensure idempotency for GET, PUT, DELETE. Comment if not respected.  
- **M-020**: Comment if incorrect or missing status codes (must follow 2xx, 4xx, 5xx conventions).  
- **M-026**: For `POST`, ensure `Idempotency-Key` header is required and documented. Comment if missing.

---

### 🧨 Error Handling

When reviewing error responses:
- **M-028**: Comment if any response leaks stack traces or internal exception details.  
- **M-043**: Ensure all errors follow this structure:
  ```json
  {
    "error": {
      "code": "ERROR_CODE",
      "message": "Human-readable description",
      "details": [
        {"field": "field_name", "code": "error_code", "message": "detail"}
      ],
      "trace_id": "correlation-id"
    }
  }
  ```
  Comment if any deviation is found.

---

### 📬 Headers

When reviewing HTTP headers:
- **M-044**: Comment if non-approved proprietary headers are used.  
  Allowed headers:
  - `X-Request-ID` (required)  
  - `X-Correlation-ID` (optional)  
  - `X-Tenant-ID` (conditional)  
  - `X-Source-System` (required)  
  - `X-Original-Request-Time` (required)
- **M-045**: Comment if these headers are not properly propagated in service-to-service calls.

---

### 📅 Data Formats

- **M-023**: Comment if media type is not `application/json`.  
- **M-024**: Comment if date/time fields are not in ISO 8601 format (e.g., `2025-04-17T14:30:00Z`).

---

## ⚠️ SHOULD Requirements (Strong Recommendations — Add Suggestions)

### 🧱 Data Structure

When reviewing response or request schemas:
- **S-006**: Comment if booleans can be `null`. Suggest using `true`/`false` or omitting the field.  
- **S-007**: Suggest avoiding `null` values entirely.  
- **S-009**: Comment if empty arrays are represented as `null`; recommend using `[]`.  
- **S-010**: Suggest defining enumerations as uppercase strings (e.g., `UPPER_SNAKE_CASE`).

---

### 🔧 API Design (Recommended)

- **S-002**: Suggest only adding optional fields for backward compatibility.  
- **S-014**: Comment if API defines more than 8 resources; recommend splitting.  
- **S-015**: Comment if path nesting exceeds 3 levels.  
- **S-019**: Recommend cursor-based pagination for collection endpoints.  
- **S-022**: Comment if `Location` header is missing from 201 Created responses.

---

## ✅ Quick Review Checklist

### Critical (MUST)
- [ ] OpenAPI spec exists and is current (**M-002**)  
- [ ] Properties use `snake_case` (**M-010**)  
- [ ] Paths use `kebab-case` (**M-011**)  
- [ ] Resources are plural (**M-013**)  
- [ ] POST endpoints include `Idempotency-Key` (**M-026**)  
- [ ] Error format matches standard (**M-043**)  
- [ ] No stack traces returned (**M-028**)  
- [ ] Backward compatibility maintained (**M-007**)  
- [ ] Only approved headers used (**M-044**)

### Recommended (SHOULD)
- [ ] Booleans and arrays not null (**S-006**, **S-009**)  
- [ ] ≤8 resources, ≤3 sublevels (**S-014**, **S-015**)  
- [ ] Cursor-based pagination (**S-019**)  
- [ ] `Location` header for 201 responses (**S-022**)

---

## 🤖 Notes for Copilot

- Comment directly in diff context when possible.  
- If a rule may apply but evidence is incomplete, phrase it as:
  > Possible issue: This endpoint might not include an `Idempotency-Key`. Please verify.
- Focus on **API correctness, naming, and structure**, not internal logic or formatting.
- Be concise and consistent in tone.

---

**End of Instructions**
