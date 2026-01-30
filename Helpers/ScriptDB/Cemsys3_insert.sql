-- INSERT para CantidadCuotas (según el ejemplo proporcionado)
INSERT INTO CantidadCuotas (cuota) VALUES 
(1),
(2),
(3),
(4),
(5),
(6);

-- INSERT para TiposConceptoTarifaria
INSERT INTO [TemasTarifaria] (nombre) VALUES 
('General'),
('Inhumación'),
('Concesión nicho'),
('Concesión fosa'),
('Registro Civil'),
('Derecho de Oficina'),
('Fondo');

-- INSERT para RolesUsuarios
INSERT INTO RolesUsuarios (rol) VALUES 
('Empleado'),
('Administrador');

-- INSERT para TipoNumeracionParcelas
INSERT INTO TipoNumeracionParcelas (tipoNumeracion) VALUES 
('Nueva (nichos repetidos)'),
('Antigua (sin repetir)');

-- INSERT para TipoNichos
INSERT INTO TipoNichos (tipo) VALUES 
('Féretro'),
('Urnario'),
('Especial');

-- INSERT para AniosConcesion (años típicos de concesión)
INSERT INTO AnioConcesion (anios) VALUES 
(1),
(5),
(10),
(15),
(25);

-- INSERT para CategoriaPersonas
INSERT INTO CategoriasPersonas (categoria) VALUES 
('Titular'),
('Fallecido');

INSERT INTO TipoNota (Descripcion, visibilidad) VALUES 
('Ingreso', 1);

-- INSERT para EstadoDifunto
INSERT INTO EstadosDifunto (estado) VALUES 
('Cuerpo completo'),
('Reducido'),
('Cremado');

-- INSERT para TipoTramite
INSERT INTO TipoTramite (tipo) VALUES 
('Ingreso'), --1
('Autorización para cremación'), --2
('Autorización para reducción'), --3
('Contrato de concesión'), --4
('Autorización para traslado'), --5
('Cambio de titularidad'), --6
('Nota'); --7

-- INSERT para TipoParcela
INSERT INTO TipoParcela (tipo) VALUES 
('Nicho'),
('Fosa'),
('Panteón');

-- INSERT para TipoPanteon
INSERT INTO TipoPanteon (tipo) VALUES 
('Con nichos'),
('Sin nichos');

-- INSERT para conceptos
INSERT INTO ConceptosTarifaria (nombre, temaId) VALUES
('Apertura de nicho con placa', 1), --1
('Apertura de nicho sin placa', 1), --2
('Apertura de fosa', 1), --3
('Cierre de nicho', 2), --4
('Cierre de fosa', 2), --5
('Permiso para colocar placa', 1), --6
('Permiso de refacciones', 1), --7
('Reducción', 1),--8
('Cremación', 1),--9
('Inhumación nicho', 2),--10
('Inhumación fosa', 2),--11
('Inhumación panteón', 2),--12
('Defunción', 5),--13
('Transcripción de acta', 5),--14
('Introducción', 6),--15
('Concesión Nicho', 3),--16
('Concesión Fosa', 4),--17
('% de fondo de ayuda centro de salud', 7),--18
('Monto mínimo de fondo', 7),--19
('% de aumento de inhumación de otras localidades',7), --20
('% de aumento de concesiones de otras localidades',7), --21
('% de aumento de introducción derecho de oficina de otras localidades',7), --22
('% de precios de nichos urnarios de secc 16-18',7), --23
('% de introducción de urna de derecho de oficina',7); --24





--INSERT PreciosTarifaria
INSERT INTO PreciosTarifarias (precio, conceptoTarifariaId, visibilidad) values 
(0.00, 1, 1),
(0.00, 2, 1),
(0.00, 3, 1),
(0.00, 4, 1),
(0.00, 5, 1),
(0.00, 6, 1),
(0.00, 7, 1),
(0.00, 8, 1),
(0.00, 9, 1),
(0.00, 10, 1),
(0.00, 11, 1),
(0.00, 12, 1),
(0.00, 13, 1),
(0.00, 14, 1),
(0.00, 15, 1),
(0.05, 18, 1), --fondo %
(0.00, 19, 1), --monto minimo fondo
(1.00, 20, 1), --% de aumento de inhumación de otras localidades
(0.50, 21, 1), --% de aumento de concesiones de otras localidades
(1.00, 22, 1), --% de aumento de derecho de oficina de otras localidades
(0.50, 23, 1), --% de precios de nichos urnarios de secc 16-18
(0.25, 24, 1); --% de precios de nichos urnarios de secc 16-18

--para fosas
INSERT INTO PreciosTarifarias (precio, conceptoTarifariaId, visibilidad, nroFila, aniosConcesionId) values 
(0.00, 17, 1, 1, 4), --15 años
(0.00, 17, 1, 1, 5); --25 años


INSERT INTO EstadosTramites (estado, tipoTramiteId)
VALUES 
('Registrado', 1), --ingreso
('Finalizado', 1), --ingreso
('Pendiente', 7), --nota
('Finalizado', 7); --nota
--(4, 'Iniciado'), --contrato de conescion
--(4, 'Pendiente de documentación'),
--(4, 'Activa'),
--(4, 'Vencida'),
--(4, 'Inactiva'),
--(4, 'Renovación');


insert into Personas 
(nombre, visibilidad, sexo) values ('Municipalidad Colonia Tirolesa', 1, 'otro');

INSERT INTO Usuarios (nombre, apellido, correo, usuario, clave, rolId) values ('Tomas', 'Carreras', 'tomaselle2@gmail.com', 'Tomaselle2', 'P7eSe/VyhW8UaKMx5qghSw==.JYrOO0ZJQLp0A82FUreiYz7mWl+BpZykU1AfM1ZOpZU=', 2);



