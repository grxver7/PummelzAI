using System.Collections.Generic;

namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumSourceFieldFilter : MGPumFilter
    {

        public MGPumSourceFieldFilter() : base()
        {
            
        }

         protected override List<MGPumEntity> applyBaseFilter(List<MGPumEntity> entities, MGPumEntity source)
        {
            List<MGPumEntity> filteredEntities = new List<MGPumEntity>();

            foreach (MGPumEntity entity in entities)
            {
                if (entity.id  == source.id && entity is MGPumUnit && ((MGPumUnit)entity).field != null)
                {
                    filteredEntities.Add(((MGPumUnit)entity).field);
                }
            }
            return filteredEntities;
        }

        protected override List<MGPumEntity> applyBaseFilter(MGPumGameState state, MGPumEntity source)
        {
            List<MGPumEntity> filteredEntities = new List<MGPumEntity>();
            if (source is MGPumUnit && ((MGPumUnit)source).field != null)
            {
                filteredEntities.Add(((MGPumUnit)source).field);
            }
            return filteredEntities;
        }

        public override MGPumSelection deepCopy(MGPumGameState state)
        {
            MGPumSourceFieldFilter copy = new MGPumSourceFieldFilter();
            copyToFilter(copy);

            return copy;
        }

    }
}
