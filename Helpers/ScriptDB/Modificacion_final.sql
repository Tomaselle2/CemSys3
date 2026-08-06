use cemsys
go

--solo cuando se termines de crear las fosas
--antes de carga inicial de fallecidos, agrega la fosa 0 en seccion antig
insert into Parcelas(visibilidad, nroParcela, nroFila, cantidadDifuntos, nombrePanteon, informacionAdicional, seccionId, tipoParcelaId) 
values 
(1, 0, 1, 0, '', '', 66, 2);

update Secciones set nroParcelas = 35 where id = 66


SELECT
    c.Concesion,
    s.Nombre AS Seccion,
    p.NroFila,
    p.NroParcela,
    c.Vencimiento
FROM Concesiones c
INNER JOIN Parcelas p
    ON c.ParcelaId = p.Id
INNER JOIN Secciones s
    ON p.SeccionId = s.Id
LEFT JOIN ParcelaDifuntos pd
    ON pd.ParcelaId = p.Id
    AND pd.FechaRetiro IS NULL
WHERE pd.Id IS NULL
  AND c.FechaFin IS NULL      -- No está caducada
ORDER BY c.Concesion;