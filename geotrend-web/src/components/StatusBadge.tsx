import React from 'react';
import { CheckCircle, AlertTriangle, AlertCircle, ShieldAlert } from 'lucide-react';

interface StatusBadgeProps {
    status: string;
}

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
    const getStyles = () => {
        switch (status) {
            case 'Normal':
                return { bg: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20', icon: CheckCircle, label: 'Normal' };
            case 'Atencao':
                return { bg: 'bg-amber-500/10 text-amber-400 border-amber-500/20', icon: AlertTriangle, label: 'Atenção' };
            case 'Alerta':
                return { bg: 'bg-orange-500/10 text-orange-400 border-orange-500/20', icon: AlertCircle, label: 'Alerta' };
            case 'Emergencia':
                return { bg: 'bg-rose-500/10 text-rose-400 border-rose-500/20 animate-pulse', icon: ShieldAlert, label: 'Emergência' };
            default:
                return { bg: 'bg-slate-500/10 text-slate-400 border-slate-500/20', icon: CheckCircle, label: status };
        }
    };

    const { bg, icon: Icon, label } = getStyles();

    return (
        <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold border ${bg}`}>
      <Icon className="w-3.5 h-3.5" />
            {label}
    </span>
    );
};