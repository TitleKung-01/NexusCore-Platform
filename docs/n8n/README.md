# n8n — แจ้งเตือนอีเมล HR-Lite

HR-Lite **ไม่ส่งอีเมลจาก .NET โดยตรง** — API โพสต์ JSON ไปที่ n8n webhook แล้วให้ workflow ส่งเมล (SMTP / Gmail ฯลฯ)

## รัน n8n

### Docker (`make docker-up`)

- UI: http://localhost:5678
- Webhook URL (ใน compose): `http://n8n:5678/webhook/hr-events`

### Local dev (`make dev`)

1. รัน n8n แยก หรือ `docker compose up n8n -d`
2. ตั้งค่าใน [`src/NexusCore.Api/appsettings.Development.json`](../src/NexusCore.Api/appsettings.Development.json):

```json
"N8n": {
  "WebhookUrl": "http://localhost:5678/webhook/hr-events",
  "WebhookSecret": "dev-secret"
}
```

## สร้าง workflow

1. เปิด n8n → **New workflow**
2. เพิ่ม node **Webhook**
   - HTTP Method: POST
   - Path: `hr-events`
   - Authentication (optional): Header `X-HR-Lite-Secret` = ค่าเดียวกับ `N8n__WebhookSecret`
3. เพิ่ม **Switch** ตาม `{{ $json.eventType }}`
4. แต่ละ branch → **Send Email** (ตั้ง SMTP credentials ใน n8n)

## Payload ตัวอย่าง

```json
{
  "eventType": "leave.submitted",
  "recipientEmail": "manager@example.com",
  "subject": "คำขอลาใหม่รออนุมัติ",
  "body": "สมหญิง ยื่นคำขอลา 1–3 มิ.ย.",
  "metadata": { "userId": "...", "linkPath": "/leave/..." }
}
```

## Event types

| eventType | เมื่อไหร่ |
|-----------|----------|
| `leave.submitted` | ส่งคำขอลา |
| `leave.approved` | อนุมัติลา |
| `leave.rejected` | ปฏิเสธลา |
| `leave.cancelled` | ยกเลิกลา |
| `overtime.submitted` / `approved` / `rejected` | OT |
| `expense.submitted` / `decided` | เบิกค่าใช้จ่าย |
| `announcement.published` | ประกาศ HR |
| `payslip.published` | อัปโหลดสลิป |
| `onboarding.assigned` | มอบหมาย onboarding |
| `review.opened` | เปิดรอบประเมิน |

## Import template

นำเข้า [`hr-lite-events.workflow.json`](./hr-lite-events.workflow.json) แล้วเปิดใช้ workflow (Activate) ก่อนทดสอบ
