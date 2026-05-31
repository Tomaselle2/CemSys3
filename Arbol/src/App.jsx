import { useEffect, useCallback } from "react";
import PersonaNode from "./PersonaNode";

import {
    ReactFlow,
    Background,
    Controls,
    addEdge,
    useNodesState,
    useEdgesState,
    ConnectionLineType
} from "@xyflow/react";

import "@xyflow/react/dist/style.css";

export default function App() {
    const nodeTypes = {
        persona: PersonaNode
    };

    const root = document.getElementById("react-flow-root");

    const tramiteId = root.dataset.tramiteId;

    const [nodes, setNodes, onNodesChange] = useNodesState([]);

    const [edges, setEdges, onEdgesChange] = useEdgesState([]);

    const onConnect = useCallback(
        (params) => setEdges((eds) => addEdge(params, eds)),
        []
    );

    const agregarPersona = () => {

        const nuevoNodo = {

            id: crypto.randomUUID(),

            type: 'persona',

            position: {
                x: 250,
                y: 250
            },

            data: {
                label: '',
                backgroundColor: 'blanco',
                onChange: actualizarTextoNodo,
                onColorChange: actualizarColorNodo
            }
        };

        setNodes((nds) => [...nds, nuevoNodo]);
    };

    const actualizarTextoNodo = (id, value) => {

        setNodes((nds) =>
            nds.map((node) => {

                if (node.id === id) {

                    node.data = {
                        ...node.data,
                        label: value
                    };
                }

                return node;
            })
        );
    };

    const actualizarColorNodo = (id, color) => {

        setNodes((nds) =>
            nds.map((node) => {

                if (node.id === id) {

                    node.data = {
                        ...node.data,
                        backgroundColor: color
                    };
                }

                return node;
            })
        );
    };

    useEffect(() => {

        fetch(`/Arbol/ObtenerDiagrama?tramiteId=${tramiteId}`)
            .then(r => r.json())
            .then(data => {

                if (data.jsonDiagrama) {

                    const flow = JSON.parse(data.jsonDiagrama);

                    const nodesConEventos = (flow.nodes || []).map(node => ({
                        ...node,
                        data: {
                            ...node.data,
                            backgroundColor: node.data.backgroundColor || 'blanco',
                            onChange: actualizarTextoNodo,
                            onColorChange: actualizarColorNodo
                        }
                    }));

                    setNodes(nodesConEventos);
                    setEdges(flow.edges || []);
                }
            });

    }, []);

    const guardar = async () => {

        const flow = {

            nodes: nodes.map(n => ({
                ...n,
                data: {
                    ...n.data,
                    onChange: undefined,
                    onColorChange: undefined
                }
            })),

            edges
        };

        await fetch('/Arbol/GuardarDiagrama', {

            method: 'POST',

            headers: {
                'Content-Type': 'application/json'
            },

            body: JSON.stringify({

                tramiteId,
                jsonDiagrama: JSON.stringify(flow)
            })
        });

        AlertService.show("Éxito", "Guardado correctamente", "success");
    };


    return (
        <div style={{ width: '100%', height: '700px' }}>

            <button className="btn btn-info m-2" onClick={agregarPersona}>
                👤 Persona
            </button>

            <button className="btn btn-success m-2" onClick={guardar}>
                Guardar
            </button>

            {/* Referencias de colores */}
            <div style={{
                display: 'inline-flex',
                alignItems: 'center',
                gap: '12px',
                marginLeft: '8px',
                padding: '4px 12px',
                backgroundColor: '#f8f9fa',
                borderRadius: '20px',
                fontSize: '14px'
            }}>
                <span style={{ fontWeight: '500', marginRight: '4px' }}>Colores:</span>

                <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <div style={{
                        width: 14,
                        height: 14,
                        borderRadius: '50%',
                        backgroundColor: '#ef4444',
                        border: '1px solid #ccc'
                    }} />
                    <span>Fallecido</span>
                </div>

                <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <div style={{
                        width: 14,
                        height: 14,
                        borderRadius: '50%',
                        backgroundColor: '#3b82f6',
                        border: '1px solid #ccc'
                    }} />
                    <span>Firmante</span>
                </div>

                <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <div style={{
                        width: 14,
                        height: 14,
                        borderRadius: '50%',
                        backgroundColor: '#e5e7eb',
                        border: '1px solid #ccc'
                    }} />
                    <span>Familiar</span>
                </div>
            </div>

            <ReactFlow
                nodes={nodes}
                edges={edges}
                nodeTypes={nodeTypes}
                onNodesChange={onNodesChange}
                onEdgesChange={onEdgesChange}
                onConnect={onConnect}

                deleteKeyCode={["Delete", "Backspace"]}

                connectionLineType="smoothstep"

                defaultEdgeOptions={{
                    type: 'smoothstep',
                    style: {
                        strokeWidth: 2
                    }
                }}
                fitView
            >
                <Background />
                <Controls />
            </ReactFlow>

        </div>
    );
}