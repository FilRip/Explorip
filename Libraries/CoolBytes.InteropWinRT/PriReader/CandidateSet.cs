using System.Collections.Generic;

namespace CoolBytes.InteropWinRT.PriReader;

public class CandidateSet
{
    public ResourceMapItemRef ResourceMapItem { get; }

    public ushort DecisionIndex { get; }

    public IReadOnlyList<Candidate> Candidates { get; }

    internal CandidateSet(ResourceMapItemRef resourceMapItem, ushort decisionIndex, IReadOnlyList<Candidate> candidates)
    {
        ResourceMapItem = resourceMapItem;
        DecisionIndex = decisionIndex;
        Candidates = candidates;
    }
}
