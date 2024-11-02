### Мини документация по API [ЭлЖур](https://eljur.ru)

## Для всех запросов требуются файлы cookie

Возвращает JSON с электронными письмами
```url
https://kip.eljur.ru/journal-api-messages-action?method=messages.get_list&category=inbox&search=&limit=20&offset=0&teacher=0&status=&companion=&minDate=0
```
Параметры:
* search - поиск
* limit - количество отображаемых писем
* offset - отсутствие в отображении писем, например, чтобы перейти на вторую страницу при лимите 20, offset будет равен 20
* teacher - дентификатор учителя, под которым была произведена авторизация (может быть 0)
* status - статус письма, оно может быть read или unread

Пример возвращаемого JSON:
```json
{
	"category": "inbox",
	"search": "",
	"limit": 20,
	"offset": 0,
	"total": 5271,
	"list": [
		{
			"subject": "Практическая_работа_№7",
			"msg_date": "2024-10-29 18:30:19",
			"from_user": "16510",
			"id": "3683070",
			"status": "unread",
			"files": [
				{
					"id": 3672934,
					"fid": "f1caa9bf5f835821ee9aa530d4597351",
					"created_at": "2024-10-29 00:00:00",
					"source_id": 3683070,
					"username": null,
					"filename": "Практическая_работа_№7_.ipynb",
					"source_type": 8,
					"url": "https://storage14.eljur.ru/storage/"
				}
			],
			"hasFiles": true,
			"resources": [],
			"hasResources": false,
			"messageDateHuman": "18:30",
			"fromUserHuman": "Фамилия Имя"
		},
		{
			"subject": "Практическая работа №7",
			"msg_date": "2024-10-29 18:28:18",
			"from_user": "16518",
			"id": "3683034",
			"status": "unread",
			"files": [
				{
					"id": 3672902,
					"fid": "188ecd31131714646ffdb09676916f8d",
					"created_at": "2024-10-29 00:00:00",
					"source_id": 3683034,
					"username": null,
					"filename": "Практическая_работа_№7.ipynb",
					"source_type": 8,
					"url": "https://storage14.eljur.ru/storage/"
				}
			],
			"hasFiles": true,
			"resources": [],
			"hasResources": false,
			"messageDateHuman": "18:28",
			"fromUserHuman": "Фамилия Имя"
		}
	],
	"companion": 0,
	"canSeeAnotherMessages": false,
	"teachers": [],
	"teacher": 21742,
	"currentTeacher": 21742,
	"canEdit": true,
	"statuses": [
		"unread",
		"read",
		""
	],
	"status": "",
	"minDates": {
		"today": "Сегодня",
		"week": "Неделя",
		"month": "Месяц",
		"two_month": "2 месяца",
		"year": "Год"
	},
	"minDate": 0,
	"pager": {
		"url": "",
		"from": 1,
		"to": 20,
		"total": "5271",
		"pages": [
			{
				"page": 1,
				"offset": 0,
				"current": true
			},
			{
				"page": 2,
				"offset": 20,
				"current": false
			},
			{
				"page": 3,
				"offset": 40,
				"current": false
			},
			{
				"page": 4,
				"offset": 60,
				"current": false
			},
			{
				"page": 5,
				"offset": 80,
				"current": false
			}
		],
		"position": "right"
	}
}
```

Пометить сообщение как прочитанное или непрочитанное
```url
https://kip.eljur.ru/journal-api-messages-action?method=messages.note_read&idsString=3202690
```
Parameters:
* method - может быть messages.note_read (прочитано) и messages.note_unread (непрочитанно)
* idsString - ID сообщения

Пример возвращаемого JSON:
```json
{
	"result": true
}
```

Возвращает список отправителей
```url
https://kip.eljur.ru/journal-api-messages-action?method=messages.get_senders_list_by_name
```
Пример возвращаемого JSON:
```json
{
	"result": true,
	"options": [
		{
			"value": "8805",
			"label": "Фамилия Имя Отчество"
		},
		{
			"value": "8697",
			"label": "Фамилия Имя Отчество"
		},
		{
			"value": "10294",
			"label": "Фамилия Имя Отчество"
		},
		{
			"value": "9974",
			"label": "Фамилия Имя Отчество"
		}
	]
}
```

Возвращает категории получателей (необходимо для получения списка групп)
```url
https://kip.eljur.ru/journal-api-messages-action?method=messages.get_recipient_structure
```
Пример возвращаемого JSON:
```json
{
	"result": true,
	"structure": [
		{
			"key": "school",
			"name": "Образовательная организация",
			"data": [
				{
					"key": "teachers",
					"name": "Преподаватели",
					"data": []
				},
				{
					"key": "classruks",
					"name": "Кл. руководители",
					"data": []
				},
				{
					"key": "administration",
					"name": "Администрация",
					"data": []
				},
				{
					"key": "specialists",
					"name": "Специалисты",
					"data": []
				},
				{
					"key": "parents",
					"type": "parents",
					"name": "Родители",
					"data": [
						{
							"key": "2024/2025_1_Название и номер группы#####ключ группы",
							"name": "Название и номер группы",
							"dep": 1
						},
						{
							"key": "2024/2025_1_Название и номер группы#####ключ группы",
							"name": "Название и номер группы",
							"dep": 1
						}
					]
				},
				{
					"key": "students",
					"type": "children",
					"name": "Студенты",
					"data": [
						{
							"key": "2024/2025_1_Название и номер группы#####ключ группы",
							"name": "Название и номер группы",
							"dep": 1
						},
						{
							"key": "2024/2025_1_Название и номер группы#####ключ группы",
							"name": "Название и номер группы",
							"dep": 1
						}
					]
				},
				{
					"key": "categories",
					"name": "Категории",
					"data": [
						{
							"key": 2,
							"name": "Директор"
						},
						{
							"key": 6,
							"name": "Преподаватель"
						}
					]
				}
			]
		},
		{
			"key": "ext",
			"name": "Журналы",
			"data": []
		},
		{
			"key": "ugroups",
			"name": "Группы",
			"data": [
				{
					"key": "146",
					"name": "1 курс_2022_с родителями",
					"data": []
				},
				{
					"key": "150",
					"name": "ИСиП-522",
					"data": []
				}
			]
		}
	],
	"departments": [
		{
			"id": 1,
			"name": "Колледж",
			"type": "INST"
		}
	]
}
```

Возвращает список людей в группе
```url
https://kip.eljur.ru/journal-api-messages-action?method=messages.get_recipients_list&key1=school&key2=students&key3=2024%2F2025_1_3%D0%98%D0%A1%D0%98%D0%9F-122%23%23%23%23%230753a2848830c9e5f25229d379c79c7f&dep=null
```
Parameters:
* key - тут должен быть ключ группы. Состоит из параметра "key", который можно получить из запроса на получение списка всех групп

Пример возвращаемого JSON:
```json
{
	"result": true,
	"user_list": [
		{
			"id": "15158",
			"firstname": "Имя",
			"lastname": "Фамилия",
			"middlename": "Отчество",
			"info": "",
			"sub_user_list": [
				{
					"id": "15162",
					"firstname": "Имя",
					"lastname": "Фамилия",
					"middlename": "Отчество",
					"info": ""
				}
			]
		},
		{
			"id": "15166",
			"firstname": "Имя",
			"lastname": "Фамилия",
			"middlename": "Отчество",
			"info": "",
			"sub_user_list": [
				{
					"id": "15170",
					"firstname": "Имя",
					"lastname": "Фамилия",
					"middlename": "Отчество",
					"info": ""
				}
			]
		}
	]
}
```
