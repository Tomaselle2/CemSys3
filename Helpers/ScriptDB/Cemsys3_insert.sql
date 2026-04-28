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
('Ingreso', 1),
('Recordatorio', 1);

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
('Nota'), --7
('Aceptación de titularidad'); --8

INSERT INTO RequisitosTramite (tipoTramiteId, descripcion)
VALUES 
(8, '- Se necesita que esté presente el titular el nuevo titular.  - El trámite es sin costo.   - De lunes a viernes de 7:00hs a 12:30hs.'), --aceptacion de titularidad
(2, 'Adjuntar acta de defunción si el titular está fallecido'),
(3, 'Presentar libreta de familia'),
(4, 'Adjuntar acta de defunción si el titular está fallecido'),
(5, 'Adjuntar acta de defunción si el titular está fallecido'),
(6, '- Se necesita que esté presente el titular ({TitularesActuales}) y el nuevo titular.  - El trámite es sin costo.   - De lunes a viernes de 7:00hs a 12:30hs.'); --cambio de titular


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
('% de introducción de urna de derecho de oficina',7),--24
('% de descuento de renovacion de concesión al dia',7); --25


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
(1000.00, 19, 1), --monto minimo fondo
(1.00, 20, 1), --% de aumento de inhumación de otras localidades
(0.50, 21, 1), --% de aumento de concesiones de otras localidades
(1.00, 22, 1), --% de aumento de introduccion derecho de oficina de otras localidades
(0.50, 23, 1), --% de precios de nichos urnarios de secc 16-18
(0.25, 24, 1), --% de introduccion de urna de derecho de oficina
(0.30, 25, 1); --% de descuento de renovacion de concesión al dia

--para fosas
INSERT INTO PreciosTarifarias (precio, conceptoTarifariaId, visibilidad, nroFila, aniosConcesionId) values 
(0.00, 17, 1, 1, 4), --15 años
(0.00, 17, 1, 1, 5); --25 años


INSERT INTO EstadosTramites (estado, tipoTramiteId)
VALUES 
('Registrado', 1), --ingreso 1
('Finalizado', 1), --ingreso 2
('Pendiente', 7), --nota 3
('Finalizado', 7), --nota 4
('SinContrato', 4), --contrato de concesion 5
('Vigente', 4), --contrato de concesion 6
('Vencido', 4), --contrato de concesion 7
('Caducado', 4), --contrato de concesion 8
('Iniciado', 6),
('Finalizado', 6), 
('Cancelado', 6); 

--insert into EstadosTramites (estado, tipoTramiteId) values ('Iniciado', 6), ('Finalizado', 6);
INSERT INTO Usuarios (nombre, apellido, correo, usuario, clave, rolId) values ('Tomas', 'Carreras', 'tomaselle2@gmail.com', 'Tomaselle2', 'P7eSe/VyhW8UaKMx5qghSw==.JYrOO0ZJQLp0A82FUreiYz7mWl+BpZykU1AfM1ZOpZU=', 2);

--precios de nichos
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Fallecidos de otra localidad con domicilio en otra localidad',
1, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Fallecidos de otra localidad con domicilio en Tirolesa',
1, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Fallecidos en Colonia Tirolesa',
1, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Cenizas en urnas con domicilio en Tirolesa',
1, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Cenizas en urnas con domicilio en otra localidad',
1, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

--precios de fosas
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Fallecidos de otra localidad con domicilio en otra localidad',
2, --tipoParcelaId--fosa
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
5, --cierre fosa --si aplica
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Fallecidos de otra localidad con domicilio en Tirolesa',
2, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
5, --cierre fosa --si aplica
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Fallecidos en Colonia Tirolesa',
2, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
5, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Cenizas en urnas con domicilio en Tirolesa',
2, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Cenizas en urnas con domicilio en otra localidad',
2, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

--precios panteon con nichos
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Fallecidos de otra localidad con domicilio en otra localidad',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId --con nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Fallecidos de otra localidad con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId --con nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Fallecidos en Colonia Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Cenizas en urnas con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Cenizas en urnas con domicilio en otra localidad',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

--precios panteon sin nichos
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Fallecidos de otra localidad con domicilio en otra localidad',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId --sin nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Fallecidos de otra localidad con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId --sin nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Fallecidos en Colonia Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Cenizas en urnas con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Cenizas en urnas con domicilio en otra localidad',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad


Insert into TipoAutorizacion (tipoTramiteId, nombre) values 
(6, 'Cambio titular - ambos presentes'),
(8, 'Aceptación de titularidad'),
(2, 'Cremación - Autorización'),
(2, 'Cremación - Libre Tránsito');

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(6, 'Cambio Titular - Ambos Presentes',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p style="text-align: justify;"> </p>  <p style="text-align: justify;">Se deja constancia de que {articuloTitularActual} {sr/sraTitularActual} <strong>{TitularesActuales} DNI {DniTitularActual}, </strong>quien suscribiera el contrato de concesión real de uso sobre <strong>{Parcela} </strong>el cual está ocupado por <strong>{Difuntos},</strong> sito en el cementerio Municipal de Colonia Tirolesa cede {articuloNuevoTitular} {sr/sraNuevoTitular} <strong>{NuevosTitulares} DNI {DniNuevosTitulares}. </strong>La concesión del bien mencionado supra de común entre las partes presentes.<br> </p>  <br><br><br><br><br><br><br><br>  <p class="firma-linea">FIRMA_____________________________________                 DNI___________________________________</p>  <br><br><br>  <p class="firma-linea">FIRMA_____________________________________                 DNI___________________________________</p>  <br><br><br><br><br><br><br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png" width="700" height="100"></figure>  </div>',
1, 1); 

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(8, 'Aceptación de titularidad',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p style="text-align: justify;"> </p>  <p style="text-align: justify;">Se deja constancia de que {articuloTitularActual} {sr/sraTitularActual} <strong>{TitularesActuales} DNI {DniTitularActual}, </strong>quien suscribiera el contrato de concesión real de uso sobre <strong>{Parcela} </strong>el cual está ocupado por <strong>{Difuntos},</strong> sito en el cementerio Municipal de Colonia Tirolesa cede {articuloNuevoTitular} {sr/sraNuevoTitular} <strong>{NuevosTitulares} DNI {DniNuevosTitulares}. </strong>La concesión del bien mencionado supra de común entre las partes presentes.<br> </p>  <br><br><br><br><br><br><br><br>  <p class="firma-linea">FIRMA_____________________________________                 DNI___________________________________</p>  <br><br><br>  <p class="firma-linea">FIRMA_____________________________________                 DNI___________________________________</p>  <br><br><br><br><br><br><br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png" width="700" height="100"></figure>  </div>',
2, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(2, 'Cremación - Autorización',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p style="text-align: justify;">Atentamente:<br>El que suscribe la presente <strong>{NombreCompletoFirmante} DNI {DniFirmante}</strong> en carácter de <strong>{Parentesco}</strong> del difunto inhumado en <strong>{Parcela}</strong> ocupado por <strong>{Difuntos},</strong> <strong>({NroConcesion}),</strong> de este Cementerio Municipal; autoriza al Municipio a realizar el siguiente trámite: <strong>{AperturaNicho/Fosa}</strong> Y TRASLADO AL <strong>{crematorio} </strong>siendo el que suscribe responsable de los posibles daños que se pudieran ocasionar, fruto del trabajo realizado, (féretro, lápida, placa, etc.).<br>     Así mismo y a tales efectos declaro bajo juramento que estoy facultado para el presente requerimiento atento al vinculo expresado en la presente y que soy el único pariente legitimado, o cuento con el consentimiento de ellos. Consecuentemente haciéndome responsable exclusivo de todos los efectos que pudieran derivar de la presente.<br>A los fines de avalar los extremos indicados adjunto los siguientes comprobantes que obran en mi poder y que consisten en recibos pago mantenimiento. Como así También recibos de pago de los aranceles estipulados para dicho trámite por la ordenanza Municipal.<br>     Por cualquier eventualidad relacionada con el presente trámite fijo domicilio en <strong>{DomicilioFirmante}</strong>.</p>  <br><br><br>  <p class="firma-linea">FIRMA_____________________________________</p>  <br><br>  <p class="firma-linea">ACLARACIÓN_____________________________________</p>  <br><br>  <p class="firma-linea">TELÉFONO_____________________________________</p>  <br><br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png" width="700" height="100"></figure>  </div>',
3, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(2, 'Cremación - Libre Tránsito',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p style="text-align: justify;">Atentamente:<br>El que suscribe la presente <strong>{NombreCompletoFirmante} DNI {DniFirmante}</strong> en carácter de <strong>{Parentesco}</strong> del difunto inhumado en <strong>{Parcela}</strong> ocupado por <strong>{Difuntos},</strong> <strong>({NroConcesion}),</strong> de este Cementerio Municipal; autoriza al Municipio a realizar el siguiente trámite: <strong>{AperturaNicho/Fosa}</strong> Y TRASLADO AL <strong>{crematorio} </strong>siendo el que suscribe responsable de los posibles daños que se pudieran ocasionar, fruto del trabajo realizado, (féretro, lápida, placa, etc.).<br>     Así mismo y a tales efectos declaro bajo juramento que estoy facultado para el presente requerimiento atento al vinculo expresado en la presente y que soy el único pariente legitimado, o cuento con el consentimiento de ellos. Consecuentemente haciéndome responsable exclusivo de todos los efectos que pudieran derivar de la presente.<br>A los fines de avalar los extremos indicados adjunto los siguientes comprobantes que obran en mi poder y que consisten en recibos pago mantenimiento. Como así También recibos de pago de los aranceles estipulados para dicho trámite por la ordenanza Municipal.<br>     Por cualquier eventualidad relacionada con el presente trámite fijo domicilio en <strong>{DomicilioFirmante}</strong>.</p>  <br><br><br>  <p class="firma-linea">FIRMA_____________________________________</p>  <br><br>  <p class="firma-linea">ACLARACIÓN_____________________________________</p>  <br><br>  <p class="firma-linea">TELÉFONO_____________________________________</p>  <br><br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png" width="700" height="100"></figure>  </div>',
4, 1);



insert into TareaPlantilla (Descripcion, TipoTramiteId, Visibilidad, estado) values
('Firmar autorización', 6,1,0),
('Subir autorización firmada', 6,1,0),
('Firmar autorización', 8,1,0),
('Subir autorización firmada', 8,1,0);

--Job para pasar de concesion vigente a vencida 
--BEGIN TRANSACTION;

---- 1. Obtener trámites que pasan a vencido
--DECLARE @TramitesVencidos TABLE (
--    tramiteId INT
--);

--INSERT INTO @TramitesVencidos (tramiteId)
--SELECT t.id
--FROM Tramites t
--INNER JOIN Concesiones c ON c.tramiteId = t.id
--WHERE 
--    c.vencimiento < GETDATE()
--    AND t.estadoActualId = 6; -- Vigente


---- 2. Actualizar estado actual
--UPDATE t
--SET estadoActualId = 7 -- Vencido
--FROM Tramites t
--INNER JOIN @TramitesVencidos tv ON tv.tramiteId = t.id;


---- 3. Insertar historial
--INSERT INTO HistorialEstadoTramite (fecha, tramiteId, estadoTramiteId)
--SELECT 
--    GETDATE(),
--    tramiteId,
--    7 -- Vencido
--FROM @TramitesVencidos;


--COMMIT;
