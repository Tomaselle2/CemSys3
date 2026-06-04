import {
    Handle,
    Position
} from "@xyflow/react";

import {
    useEffect,
    useRef,
    useState
} from "react";

export default function PersonaNode({ data, id }) {

    const [texto, setTexto] = useState(data.label || "");
    const [colorFondo, setColorFondo] = useState(data.backgroundColor || "white");

    const textareaRef = useRef(null);

    useEffect(() => {

        setTexto(data.label || "");

    }, [data.label]);

    useEffect(() => {

        autoResize();

    }, [texto]);

    const autoResize = () => {

        if (!textareaRef.current)
            return;

        textareaRef.current.style.height = 'auto';

        textareaRef.current.style.height =
            textareaRef.current.scrollHeight + 'px';
    };

    const guardarCambio = () => {

        data.onChange(id, texto);
    };

    const cambiarColor = (color) => {
        setColorFondo(color);
        if (data.onColorChange) {
            data.onColorChange(id, color);
        }
    };

    // Mapeo de colores
    const colores = {
        rojo: '#fee2e2',
        celeste: '#e0f2fe',
        blanco: '#ffffff'
    };

    const coloresCirculos = {
        rojo: '#ef4444',
        celeste: '#3b82f6',
        blanco: '#e5e7eb'
    };

    return (

        <div
            style={{
                position: 'relative'
            }}
        >

            <Handle
                type="target"
                position={Position.Top}
                style={{
                    width: 10,
                    height: 10
                }}
            />

            <div
                style={{
                    background: colores[colorFondo] || colorFondo,
                    border: '1px solid #999',
                    borderRadius: 12,
                    padding: 12,
                    minWidth: 240,
                    boxShadow: '0 2px 8px rgba(0,0,0,0.08)',
                    position: 'relative'
                }}
            >

                {/* Círculos de colores */}
                <div
                    style={{
                        position: 'absolute',
                        top: 8,
                        right: 8,
                        display: 'flex',
                        gap: 6,
                        zIndex: 10
                    }}
                >
                    <div
                        onClick={() => cambiarColor('rojo')}
                        style={{
                            width: 12,
                            height: 12,
                            borderRadius: '50%',
                            backgroundColor: coloresCirculos.rojo,
                            cursor: 'pointer',
                            border: '1px solid #ccc',
                            transition: 'transform 0.2s'
                        }}
                        onMouseEnter={(e) => e.currentTarget.style.transform = 'scale(1.2)'}
                        onMouseLeave={(e) => e.currentTarget.style.transform = 'scale(1)'}
                        title="Fallecido"
                    />
                    <div
                        onClick={() => cambiarColor('celeste')}
                        style={{
                            width: 12,
                            height: 12,
                            borderRadius: '50%',
                            backgroundColor: coloresCirculos.celeste,
                            cursor: 'pointer',
                            border: '1px solid #ccc',
                            transition: 'transform 0.2s'
                        }}
                        onMouseEnter={(e) => e.currentTarget.style.transform = 'scale(1.2)'}
                        onMouseLeave={(e) => e.currentTarget.style.transform = 'scale(1)'}
                        title="Firmante"
                    />
                    <div
                        onClick={() => cambiarColor('blanco')}
                        style={{
                            width: 12,
                            height: 12,
                            borderRadius: '50%',
                            backgroundColor: coloresCirculos.blanco,
                            cursor: 'pointer',
                            border: '1px solid #ccc',
                            transition: 'transform 0.2s'
                        }}
                        onMouseEnter={(e) => e.currentTarget.style.transform = 'scale(1.2)'}
                        onMouseLeave={(e) => e.currentTarget.style.transform = 'scale(1)'}
                        title="Familiar"
                    />
                </div>

                <div
                    style={{
                        display: 'flex',
                        gap: 10,
                        alignItems: 'flex-start'
                    }}
                >

                    <div
                        style={{
                            fontSize: 28
                        }}
                    >
                        <i class="bi bi-person-fill"></i>
                    </div>

                    <textarea
                        ref={textareaRef}
                        value={texto}
                        onChange={(e) => setTexto(e.target.value)}
                        onBlur={guardarCambio}
                        rows={1}
                        style={{
                            width: '100%',
                            resize: 'none',
                            overflow: 'hidden',
                            border: 'none',
                            outline: 'none',
                            background: 'transparent',
                            fontSize: 18,
                            fontFamily: 'inherit'
                        }}
                    />

                </div>

            </div>

            <Handle
                type="source"
                position={Position.Bottom}
                style={{
                    width: 10,
                    height: 10
                }}
            />

        </div>
    );
}