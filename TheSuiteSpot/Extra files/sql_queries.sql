-- SELECT
SELECT * FROM [User] u
WHERE u.Id = 3

SELECT * FROM [User] u
WHERE u.UserName = 'admin'

SELECT * FROM Review
WHERE StarsGiven > 95

-- GROUP BY

SELECT AVG(r.StarsGiven) averageScore, ro.RoomNumber FROM Review r
JOIN Room ro ON ro.Id = r.RoomId
GROUP BY ro.RoomNumber

SELECT COUNT(*) as numberOfBookings, b.RoomId, RoomNumber FROM Booking b
JOIN Room ro ON ro.Id = b.RoomId
GROUP BY b.RoomId, ro.RoomNumber

SELECT COUNT(*) as numberOfBookings, b.RoomId, RoomNumber FROM Booking b
JOIN Room ro ON ro.Id = b.RoomId
GROUP BY b.RoomId, ro.RoomNumber
HAVING COUNT(*) < 25

-- JOIN

SELECT * FROM Review r
JOIN Room ro ON ro.Id = r.RoomId
WHERE ro.Id = 5

SELECT * FROM SystemMessage m
JOIN UserInbox ui ON ui.Id = m.UserInboxId
JOIN [User] u ON u.UserInboxId = ui.Id
WHERE m.VoucherId is not null

SELECT * FROM SystemMessage sm
JOIN UserInbox ui ON ui.Id = sm.UserInboxId
JOIN [User] u ON u.UserInboxId = ui.Id
WHERE sm.UserInboxId = 3

-- a subquery example
SELECT * FROM [User] u
WHERE u.Id IN(
	SELECT u.Id FROM Booking b
	WHERE b.Id IN(
		SELECT b.Id FROM Invoice i))

-- another subquery example
SELECT *
FROM (
    SELECT b.Id, i.Amount
    FROM Booking b
    JOIN Invoice i ON b.id = i.BookingID
) AS query
WHERE query.Amount > (SELECT AVG(Amount) FROM Invoice);