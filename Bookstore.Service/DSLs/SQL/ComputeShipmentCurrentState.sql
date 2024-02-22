SELECT
    s.ID,
    StatusID = lastEvent.NewStatusID
FROM
    Bookstore.Shipment s
    OUTER APPLY
    (
        SELECT TOP 1 *
        FROM Bookstore.ShipmentEvent se
        WHERE se.ShipmentID = s.ID
        ORDER BY se.EffectiveSince DESC
    ) lastEvent