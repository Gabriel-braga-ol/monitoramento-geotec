import { useEffect, useState } from 'react';
import { api } from './services/api';
import { StatusBadge } from './components/StatusBadge';
import { Activity, Shield, RefreshCw, AlertCircle } from 'lucide-react';

export interface InstrumentoResumo {
    id: string;
    codigo: string;
    tipo: string;
    ultimoStatus: string;
    ultimoValor: number | null;
    dataUltimaLeitura: string | null;
}

export interface BarragemStatus {
    barragemId: string;
    nomeBarragem: string;
    statusGlobal: string;
    totalInstrumentos: number;
    quantidadeNormal: number;
    quantidadeAtencao: number;
    quantidadeAlerta: number;
    quantidadeEmergencia: number;
    instrumentos: InstrumentoResumo[];
}

export interface Barragem {
    id: string;
    nome: string;
    localizacao: string;
}

export default function App() {
    const [barragens, setBarragens] = useState<Barragem[]>([]);
    const [barragemSelecionadaId, setBarragemSelecionadaId] = useState<string>('');
    const [statusResumo, setStatusResumo] = useState<BarragemStatus | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const [erroApi, setErroApi] = useState<string | null>(null);

    // 1. Carrega a lista de barragens da API
    useEffect(() => {
        setErroApi(null);
        api.get<Barragem[]>('/Barragem')
            .then((res) => {
                if (res.data && res.data.length > 0) {
                    setBarragens(res.data);
                    setBarragemSelecionadaId(res.data[0].id);
                } else {
                    setErroApi('Nenhuma barragem cadastrada no banco de dados da API.');
                }
            })
            .catch((err) => {
                console.error('Erro ao conectar com a API:', err);
                setErroApi('Não foi possível conectar à API. Verifique se a API .NET está rodando na porta 5196.');
            });
    }, []);

    // 2. Carrega o resumo da barragem selecionada
    const carregarResumo = () => {
        if (!barragemSelecionadaId) return;
        setLoading(true);
        setErroApi(null);
        api.get<BarragemStatus>(`/Barragem/${barragemSelecionadaId}/status`)
            .then((res) => setStatusResumo(res.data))
            .catch((err) => {
                console.error('Erro ao carregar resumo:', err);
                setErroApi('Erro ao obter os dados de status da barragem.');
            })
            .finally(() => setLoading(false));
    };

    useEffect(() => {
        if (barragemSelecionadaId) {
            carregarResumo();
        }
    }, [barragemSelecionadaId]);

    return (
        <div className="min-h-screen bg-slate-950 text-slate-100 p-6">
            {/* Cabeçalho */}
            <header className="max-w-7xl mx-auto flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-slate-800 pb-6 mb-8">
                <div className="flex items-center gap-3">
                    <div className="p-2.5 bg-blue-600 rounded-xl shadow-lg shadow-blue-500/20">
                        <Activity className="w-6 h-6 text-white" />
                    </div>
                    <div>
                        <h1 className="text-2xl font-bold tracking-tight">Geotrend</h1>
                        <p className="text-xs text-slate-400">Sistema de Monitoramento Geotécnico de Barragens</p>
                    </div>
                </div>

                <div className="flex items-center gap-3">
                    <select
                        value={barragemSelecionadaId}
                        onChange={(e) => setBarragemSelecionadaId(e.target.value)}
                        className="bg-slate-900 border border-slate-700 text-slate-200 text-sm rounded-lg p-2.5 focus:ring-2 focus:ring-blue-500 outline-none min-w-[220px]"
                    >
                        {barragens.length === 0 ? (
                            <option value="">Carregando barragens...</option>
                        ) : (
                            barragens.map((b) => (
                                <option key={b.id} value={b.id}>{b.nome}</option>
                            ))
                        )}
                    </select>

                    <button
                        onClick={carregarResumo}
                        className="p-2.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-lg transition-colors border border-slate-700 cursor-pointer"
                        title="Atualizar Dados"
                    >
                        <RefreshCw className={`w-4 h-4 ${loading ? 'animate-spin' : ''}`} />
                    </button>
                </div>
            </header>

            {/* Alerta de Erro Visual */}
            {erroApi && (
                <div className="max-w-7xl mx-auto mb-6 p-4 bg-rose-500/10 border border-rose-500/20 rounded-xl text-rose-400 flex items-center gap-3 text-sm">
                    <AlertCircle className="w-5 h-5 shrink-0" />
                    <span>{erroApi}</span>
                </div>
            )}

            {/* Painel Principal */}
            {statusResumo ? (
                <main className="max-w-7xl mx-auto space-y-8">
                    <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
                        <div className="md:col-span-2 bg-slate-900 border border-slate-800 p-5 rounded-2xl flex flex-col justify-between">
                            <span className="text-xs font-medium text-slate-400 uppercase tracking-wider">Status Global da Estrutura</span>
                            <div className="mt-4 flex items-center justify-between">
                                <div>
                                    <h2 className="text-xl font-bold">{statusResumo.nomeBarragem}</h2>
                                    <p className="text-xs text-slate-500 mt-1">{statusResumo.totalInstrumentos} instrumentos ativos</p>
                                </div>
                                <StatusBadge status={statusResumo.statusGlobal} />
                            </div>
                        </div>

                        <div className="bg-slate-900/60 border border-slate-800/80 p-4 rounded-2xl">
                            <span className="text-xs text-emerald-400 font-medium">Normal</span>
                            <p className="text-2xl font-bold mt-2">{statusResumo.quantidadeNormal}</p>
                        </div>
                        <div className="bg-slate-900/60 border border-slate-800/80 p-4 rounded-2xl">
                            <span className="text-xs text-amber-400 font-medium">Atenção</span>
                            <p className="text-2xl font-bold mt-2">{statusResumo.quantidadeAtencao}</p>
                        </div>
                        <div className="bg-slate-900/60 border border-slate-800/80 p-4 rounded-2xl">
                            <span className="text-xs text-rose-400 font-medium">Alerta / Emergência</span>
                            <p className="text-2xl font-bold mt-2">{statusResumo.quantidadeAlerta + statusResumo.quantidadeEmergencia}</p>
                        </div>
                    </div>

                    <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden">
                        <div className="p-5 border-b border-slate-800 flex items-center justify-between">
                            <h3 className="font-semibold text-slate-200">Instrumentos Geotécnicos</h3>
                            <Shield className="w-4 h-4 text-slate-500" />
                        </div>
                        <div className="overflow-x-auto">
                            <table className="w-full text-left text-sm text-slate-400">
                                <thead className="bg-slate-950/50 text-xs uppercase text-slate-400 border-b border-slate-800">
                                <tr>
                                    <th className="px-6 py-3.5">Código</th>
                                    <th className="px-6 py-3.5">Tipo</th>
                                    <th className="px-6 py-3.5">Última Leitura</th>
                                    <th className="px-6 py-3.5">Status</th>
                                </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-800/60">
                                {statusResumo.instrumentos?.map((inst) => (
                                    <tr key={inst.id} className="hover:bg-slate-800/30 transition-colors">
                                        <td className="px-6 py-4 font-medium text-slate-200">{inst.codigo}</td>
                                        <td className="px-6 py-4">{inst.tipo}</td>
                                        <td className="px-6 py-4">
                                            {inst.ultimoValor !== null && inst.ultimoValor !== undefined ? (
                                                <span className="text-slate-200 font-medium">{inst.ultimoValor}</span>
                                            ) : (
                                                <span className="text-slate-600 italic">Sem leituras</span>
                                            )}
                                        </td>
                                        <td className="px-6 py-4">
                                            <StatusBadge status={inst.ultimoStatus} />
                                        </td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </main>
            ) : (
                !erroApi && (
                    <div className="max-w-7xl mx-auto text-center py-20 text-slate-500">
                        <p>Carregando dados do sistema...</p>
                    </div>
                )
            )}
        </div>
    );
}