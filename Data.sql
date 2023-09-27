INSERT INTO NHAPHATHANH (MANPH, TENNPH)
VALUES
    (1, N'Microsoft'),
    (2, N'Zoom Video Communications'),
    (3, N'Google'),
	(4, N'Design Science INC'),
	(5, N'Tonec FZE'),
	(6, N'Kaspersky Lab'),
	(7, N'ESET'),
	(8, N'VMware'),
	(9, N'Spotify'),
	(10, N'OpenAI'),
	(11, N'Electronic Arts Inc'),
	(12, N'Bandicam Company'),
	(13, N'Adobe');


INSERT INTO LOAIPHANMEM (MALOAI, TENLOAI)
VALUES
    (1, N'Văn phòng'),
    (2, N'Học tập'),
    (3, N'Giải trí'),
	(4, N'Game'),
	(5, N'Diệt virus');


set dateformat dmy
go

INSERT INTO PHANMEM (MAPM, TENPM, MOTA, MANPH, NGAYPHATHANH, THOIHAN, DONVITHOIHAN, DONGIA, SOLUONG)
VALUES
    (1, N'Microsoft 365 Business Basic', N'Microsoft 365 Business Basic', 1, '09-2-2020', 1, N'Năm', 750000, 2),
    (2, N'Microsoft 365 Business Standard', N'Microsoft 365 Business Standard', 1, '09-2-2020', 1, N'Năm', 2850000, 2),
    (3, N'Microsoft 365 Business Premium', N'Microsoft 365 Business Premium', 1, '09-2-2020', 1, N'Năm', 5650000, 1),
    (4, N'Microsoft 365 Business Standard', N'Microsoft 365 Apps for Business', 1, '09-2-2020', 1, N'Năm', 2350000, 2),
    (5, N'Microsoft 365 Home', N'Microsoft 365 Home', 1, '09-2-2020', 1, N'Năm', 1550000, 5),
    (6, N'Microsoft 365 Personal', N'Microsoft 365 Personal', 1, '09-2-2020', 1, N'Năm', 1050000, 3),
    (7, N'Google Drive 100GB', N'Lưu trữ không giới hạn', 2, '27-07-2019', 3, N'Tháng', 20000, 2),
    (8, N'Youtube Primenum Personal', N'Giải trí không giới hạn', 3, '27-07-2006', 1, N'Tháng', 80000, 4),
    (9, N'Youtube Primenum Family', N'Giải trí không giới hạn', 3, '27-07-2006', 1, N'Tháng', 150000, 4),
    (10, N'Google Drive 200GB', N'Giải trí không giới hạn', 3, '27-07-2006', 1, N'Tháng', 35000, 4),
    (11, N'Google Drive 2TB', N'Giải trí không giới hạn', 3, '27-07-2006', 1, N'Tháng', 80000, 3),
    (12, N'Zoom Meeting', N'Học tập, làm việc mọi lúc', 2, '4-5-2018', 1, N'Năm', 3600000, 3),
    (13, N'Zoom Pro', N'Học tập, làm việc mọi lúc', 2, '4-5-2018', 1, N'Tháng', 300000, 3),
    (14, N'Mathtype', N'Phần mềm toán học', 4, '14-6-2016', 1, N'Năm', 1500000, 3),
    (15, N'Internet Download Manager', N'Tốc độ download nhanh nhất ', 5, '10-11-2012', 1, N'Năm', 230000, 6),
    (16, N'Kaspersky Internet Security 1PC', N'Bảo vệ bạn mọi lúc', 6, '19-1-2016', 1, N'Tháng', 300000, 3),
    (17, N'ESET Endpoint Security', N'Bảo vệ bạn mọi lúc', 7, '27-4-2014', 1, N'Tháng', 500000, 3),
    (18, N'VMware Workstation 17 Pro', N'Giả lập máy ảo', 8, '19-1-2016', 1, N'Năm', 5000000, 2),
    (19, N'VMware Workstation 17 Player', N'Giả lập máy ảo', 8, '12-3-2019', 1, N'Năm', 4000000, 1),
    (20, N'Spotify Premium', N'Nghe nhạc mọi lúc', 9, '17-9-2020', 1, N'Năm', 300000, 2),
    (21, N'Windows 11 Professional CD Key', N'Window', 1, '17-9-2020', 100, N'Năm', 5000000, 2),
    (22, N'Chat GPT Plus', N'Hỏi AI', 10, '17-9-2020', 6, N'Tháng', 600000, 2),
    (23, N'FIFA 23 (EA)', N'Fifa 23 Game', 11, '17-9-2020', 6, N'Tháng', 1200000, 1),
    (24, N'Bandicam', N'Quay màn hình', 12, '8-2-2016', 1, N'Năm', 1500000, 1),
    (25, N'Adobe Acrobat Pro 1PC', N'Adobe', 13, '19-3-2020', 100, N'Năm', 2000000, 1),
    (26, N'Windows 11 Education CD Key', N'Window', 1, '17-9-2020', 100, N'Năm', 3000000, 2);

INSERT INTO THUOCLOAIPM(MAPM, MALOAI)
VALUES
    (1, 1),
    (2, 1),
    (3, 1),
    (4, 1),
    (5, 1),
    (6, 1),
    (7, 1),
    (8, 3),
    (9, 3),
    (10, 1),
    (11, 1),
    (12, 1),
    (13, 1),
    (14, 2),
    (15, 1),
    (16, 5),
    (17, 5),
    (18, 2),
    (19, 2),
    (20, 3),
    (21, 1),
    (22, 2),
    (23, 4),
    (24, 1),
    (25, 2),
    (26, 1);


UPDATE PHANMEM
SET DONVITHOIHAN = N'Vĩnh viễn', THOIHAN = 0
WHERE TENPM LIKE N'Windows%';
