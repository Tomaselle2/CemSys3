use cemsys
go

--solo cuando se termines de crear las fosas
--antes de carga inicial de fallecidos, agrega la fosa 0 en seccion antig
insert into Parcelas(visibilidad, nroParcela, nroFila, cantidadDifuntos, nombrePanteon, informacionAdicional, seccionId, tipoParcelaId) 
values 
(1, 0, 1, 0, '', '', 66, 2);

update Secciones set nroParcelas = 35 where id = 66